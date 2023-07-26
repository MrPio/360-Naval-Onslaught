using System;
using System.Linq;
using ExtensionsFunctions;
using JetBrains.Annotations;
using Managers;
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

    private void Start()
    {
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
        if (_target is { } || !Game.CurrentTurretModel.BulletSprite.Contains("missile"))
            return;

        //Search for target
        _accumulator += Time.fixedDeltaTime;
        if (_accumulator >= NextLookup)
        {
            _accumulator = 0;
            Physics2D.OverlapCircleNonAlloc(transform.position + (transform.rotation * Vector2.right) * 1f, 1.65f,
                _collidersArray);
            foreach (var col in _collidersArray.Where(it => it is { } && it.CompareTag("ship")))
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
        if (_target is null)
            return;
        var _transform = transform;
        transform.rotation = Quaternion.Lerp((_target.transform.position - transform.position).ToQuaternion(),
            _transform.rotation, 0.9f);
        rb.velocity = _transform.right * Game.CurrentTurretModel.Speed / 100f;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Contains("ship") && !col.GetComponent<Ship>().Invincible)
        {
            col.GetComponent<Ship>().TakeDamage(Game.CurrentTurretModel.Damage);
            Instantiate(
                original: Game.CurrentTurretModel.BulletSprite.Contains("missile")?bigExplosion:explosion,
                position: transform.position,
                rotation: Quaternion.identity
            );
            Destroy(gameObject);
        }
    }
}