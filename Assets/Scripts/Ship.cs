using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using JetBrains.Annotations;
using Managers;
using Model;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ship : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;

    private static readonly int ShipDamage = Animator.StringToHash("ship_damage");
    private static readonly int ShipDestroy = Animator.StringToHash("ship_destroy");
    public static readonly List<int[]> Collisions = new();
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private List<GameObject> explosions;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject floatingTextBig, missile, empExplosion, empText, militaryPlane;
    private Sprite _missileSprite;
    private List<AudioClip> _fireClip;
    private MoneyCounter _moneyCounter;
    private AudioClip _explodeClip;
    private ShipModel _model;
    private int _health;
    private bool _hasDelay = true;
    public bool IsFreezed;
    private float _accumulator;
    [NonSerialized] public bool Invincible;
    [NonSerialized] public int CurrentIndex;
    private static readonly int MilitaryPlaneTakeoff = Animator.StringToHash("military_plane_takeoff");

    private void Awake()
    {
        _model = Game.CurrentWave.Spawn();
        spriteRenderer.sprite = Resources.Load<Sprite>(_model.Sprite);
        boxCollider.size = spriteRenderer.bounds.size;
        if (_model.ExplodeClip != null)
            _explodeClip = Resources.Load<AudioClip>(_model.ExplodeClip);
        if (_model.FireClip != null)
            _fireClip = _model.FireClip.Select(Resources.Load<AudioClip>).ToList();
        _moneyCounter = GameObject.FindWithTag("money_counter").GetComponent<MoneyCounter>();
        GetComponent<ShipPath>().Model = _model;
        var pos = MainCamera.MainCam.RandomBoundaryPoint() * 1.15f;
        transform.SetPositionAndRotation(pos, pos.ToQuaternion());
        if (_model.MissileSprite is { })
            _missileSprite = Resources.Load<Sprite>(_model.MissileSprite);

        // Custom Path for SpeedBoat
        if (_model.Name == "SpeedBoat")
            GetComponent<ShipPath>().AddPath(
                Random.Range(0, 2) == 0
                    ? new List<Vector2> { Vector2.zero }
                    : new List<Vector2>
                    {
                        Quaternion.AngleAxis(Random.Range(-20f, 20f), Vector3.forward) * pos,
                        Vector2.zero
                    }
            );
    }

    private void Start()
    {
        _health = _model.Health;
        healthBar.gameObject.SetActive(false);
        _model.StartCallback?.Invoke(gameObject);
    }

    private void Update()
    {
        if (_model.Rate <= 0.001f)
            return;
        if (!Invincible && !IsFreezed)
            _accumulator += Time.deltaTime;
        if (_hasDelay && _accumulator >= _model.Delay)
        {
            _accumulator = 0;
            _hasDelay = false;
        }
        else if (!Invincible && _accumulator >= 100f / _model.Rate)
        {
            _accumulator = 0;
            Fire();
        }
    }

    private void OnBecameVisible()
    {
        if (_hasDelay)
            _accumulator = 0;
    }

    private void Fire()
    {
        if (_model.Name == "Aircraft Carrier")
        {
            animator.SetTrigger(MilitaryPlaneTakeoff);
            return;
        }

        const float range = 0.9f;
        var currentPos = (Vector2)transform.position;
        var destination = new Vector2(
            x: Random.Range(-range, range),
            y: Random.Range(-range, range)
        );
        var newMissile = Instantiate(
            original: missile,
            position: currentPos,
            rotation: (destination - currentPos).ToQuaternion()
        ).GetComponent<Missile>();

        IEnumerator playAllSound()
        {
            foreach (var clip in _fireClip)
            {
                MainCamera.AudioSource.PlayOneShot(clip);
                yield return new WaitForSeconds(0.5f);
            }
        }

        StartCoroutine(playAllSound());

        newMissile.SetMissile(_missileSprite);
        newMissile.StartPosition = currentPos;
        newMissile.Destination = destination;
        newMissile.Damage = _model.Damage;
    }

    public void TakeDamage(int damage, bool EMP = false)
    {
        if (!Invincible && _health > 0)
        {
            _health -= damage;
            animator.SetTrigger(ShipDamage);
            healthBar.SetValue(_health / (float)_model.Health);

            if (EMP)
            {
                IsFreezed = true;
                if (empText.activeSelf)
                    empText.GetComponent<EmpText>().EMP();
                else
                    empText.SetActive(true);
                Instantiate(
                    original: empExplosion,
                    position: transform.position,
                    rotation: Quaternion.identity
                );
            }

            if (_health <= 0)
                Explode();
        }
    }

    private void Explode(bool reward = true)
    {
        if (GetComponent<ShipPath>().IsDead || Invincible) return;
        ++Game.CurrentWave.Destroyed;

        GetComponent<ShipPath>().IsDead = true;
        MainCamera.AudioSource.PlayOneShot(_explodeClip);
        animator.SetTrigger(ShipDestroy);

        // Multiple explosions
        var bounds = GetComponent<SpriteRenderer>().bounds;

        IEnumerator SpawnExplosions()
        {
            for (var i = 0; i < _model.ExplosionsCount; ++i)
            {
                var spawnPoint = bounds.GetRandomPointInBounds();
                Instantiate(explosions.RandomItem(), spawnPoint, Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
            }
        }

        StartCoroutine(SpawnExplosions());


        // Money Reward
        if (reward)
        {
            Game.Score += _model.Health;
            var floatingTextBig = Instantiate(this.floatingTextBig, GameObject.FindWithTag("canvas").transform);
            floatingTextBig.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+ {_model.Money} $";
            floatingTextBig.transform.position = transform.position + Vector3.up * 0.5f;
            Game.Money += _model.Money;
            _moneyCounter.UpdateUI();
        }

        IEnumerator scheduleDestroy()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

        StartCoroutine(scheduleDestroy());
    }

    public void End() => Destroy(gameObject);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("base"))
        {
            GameObject.FindWithTag("base").GetComponent<Base>().TakeDamage(_model.Damage);
            Explode(false);
        }
        else if (other.CompareTag("ship"))
        {
            var otherShip = other.GetComponent<Ship>();
            if (!Collisions.Exists(it => it.Contains(CurrentIndex) && it.Contains(otherShip.CurrentIndex)))
            {
                Collisions.Add(new[] { CurrentIndex, otherShip.CurrentIndex });
                GetComponent<ShipPath>().Wait = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ship"))
        {
            var otherShip = other.GetComponent<Ship>();
            Collisions.RemoveAll(it => it.Contains(CurrentIndex) && it.Contains(otherShip.CurrentIndex));
            GetComponent<ShipPath>().Wait = false;
        }
    }

    public void SpawnMilitaryPlane()
    {
        Instantiate(militaryPlane);
    }
}