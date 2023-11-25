using System;
using System.Collections;
using System.Linq;
using ExtensionsFunctions;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject bigExplosion;
    private float _accumulator;
    private const float NextLookup = 0.2f;
    [CanBeNull] private GameObject _target = null;
    private readonly Collider2D[] _collidersArray = new Collider2D[10];
    private bool _isDestroying;
    private bool isFrozen;

    public bool IsFrozen
    {
        get => isFrozen;
        set
        {
            isFrozen = value;
            if (value)
            {
                backupVelocity = rb.velocity;
                rb.velocity = Vector2.zero;
            }
            else
            {
                if (backupVelocity is { })
                    rb.velocity = (Vector2)backupVelocity;
                backupVelocity = null;
            }
        }
    }

    private Vector2? backupVelocity;

    private void Start()
    {
        // 0.030 ms
        var sprite = Game.CurrentTurretModel.BulletSprite;
        sr.sprite = Resources.Load<Sprite>(sprite);
        transform.Find("fire").gameObject.SetActive(sprite.Contains("missile"));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (transform.rotation * Vector2.right) * 1f
            , 1.65f);
    }

    private void FixedUpdate()
    {
        if (_target is { } || !(Game.CurrentTurretModel.BulletSprite.Contains("missile") ||
                                Game.CurrentTurretModel.Name.Contains("Auto-Locking")))
            return;

        //Search for target
        _accumulator += Time.fixedDeltaTime;
        if (_accumulator >= NextLookup)
        {
            _accumulator = 0;
            Physics2D.OverlapCircleNonAlloc(transform.position + (transform.rotation * Vector2.right) * 1f, 1.65f,
                _collidersArray);
            foreach (var col in _collidersArray.Where(it => it is { } && !it.IsDestroyed() && it.CompareTag("ship")))
            {
                if (!col.GetComponent<Ship>().Invincible)
                {
                    _target = col.gameObject;
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (gameObject.IsDestroyed() || _target is null || _target.IsDestroyed() || IsFrozen)
            return;
        var _transform = transform;
        transform.rotation = Quaternion.Lerp((_target.transform.position - transform.position).ToQuaternion(),
            _transform.rotation, 0.9f);
        rb.velocity = _transform.right * Game.CurrentTurretModel.Speed / 100f;
    }

    private void OnBecameInvisible()
    {
        if (!_isDestroying)
            StartCoroutine(WaitAndDestroy());
        _isDestroying = true;
    }
    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.gameObject.tag.Contains("ship") && !col.GetComponent<Ship>().Invincible) ||
            col.gameObject.tag.Contains("bubble"))
        {
            col.GetComponent<IDamageable>().TakeDamage(Game.CurrentTurretModel.Damage,critical:Game.IsTurretCritical);
            ++GameManager.Instance.CurrentWaveTurretHit;

            Instantiate(
                original: Game.CurrentTurretModel.BulletSprite.Contains("missile") ? bigExplosion : explosion,
                position: transform.position,
                rotation: Quaternion.identity
            );
            _isDestroying = true;
            Destroy(gameObject);
        }
    }
}