using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using Model;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ship : MonoBehaviour, IDamageble
{
    private static GameManager Game => GameManager.Instance;

    private static readonly int ShipDamage = Animator.StringToHash("ship_damage");
    private static readonly int ShipDestroy = Animator.StringToHash("ship_destroy");
    public static readonly List<int[]> Collisions = new();
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private List<GameObject> explosions;
    [SerializeField] private Animator animator, foamAnimator;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public BoxCollider2D boxCollider;
    [SerializeField] private GameObject floatingTextBig, missile, empExplosion, empText, militaryPlane, bubble;
    [SerializeField] private AudioClip empHitClip;
    private Sprite _missileSprite;
    private List<AudioClip> _fireClip;
    private MoneyCounter _moneyCounter, _scoreCounter;
    private AudioClip _explodeClip;
    private ShipModel _model;
    private int _health;
    private bool _hasDelay = true;
    [NonSerialized] public bool IsFreezed;
    private float _accumulator;
    [NonSerialized] public bool Invincible = true;
    [NonSerialized] public int CurrentIndex = -1;
    private float _randomAdditionalDelay;
    private static readonly int MilitaryPlaneTakeoff = Animator.StringToHash("military_plane_takeoff");
    [NonSerialized] public bool isVisible;
    private bool _withBaseCollided;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip specialAudioClip;
    [SerializeField] private GameObject glow;
    [SerializeField] private bool alwaysSpecial;
    private bool isSpecialShip;

    private void Awake()
    {
        _model = Game.IsSpecialWave ? DataManager.Instance.Ships[0] : Game.CurrentWave.Spawn();
        spriteRenderer.sprite = Resources.Load<Sprite>(_model.Sprite);
        if (_model.ExplodeClip != null)
            _explodeClip = Resources.Load<AudioClip>(_model.ExplodeClip);
        if (_model.FireClip != null)
            _fireClip = _model.FireClip.Select(Resources.Load<AudioClip>).ToList();
        _moneyCounter = GameObject.FindWithTag("money_counter").GetComponent<MoneyCounter>();
        _scoreCounter = GameObject.FindWithTag("score_counter").GetComponent<MoneyCounter>();
        GetComponent<ShipPath>().Model = _model;
        if (_model.MissileSprite is { })
            _missileSprite = Resources.Load<Sprite>(_model.MissileSprite);

        // Custom Path for SpeedBoat

        if (_model.Name == "SpeedBoat")
        {
            var specialBoat = !Game.IsSpecialWave && !Game.SpecialOccurInWave[Game.Wave] &&
                              (alwaysSpecial || Random.Range(0f, 1f) < .08f);
            GetComponent<ShipPath>().AddPath(
                Game.IsSpecialWave || specialBoat || Random.Range(0, 2) == 0
                    ? new List<Vector2> { Vector2.zero }
                    : new List<Vector2>
                    {
                        Quaternion.AngleAxis(Random.Range(-20f, 20f), Vector3.forward) * transform.position,
                        Vector2.zero
                    }
            );
            if (specialBoat)
            {
                audioSource.PlayOneShot(specialAudioClip);
                GetComponent<ShipPath>().SpeedSpecialMultiplier = 3.75f;
                animator.SetTrigger("special_ship");
                glow.SetActive(true);
                isSpecialShip = true;
            }
        }

        _randomAdditionalDelay = Random.Range(0f, 1f);
        foamAnimator.Play(_model.FoamAnim);
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
        if (_hasDelay && _accumulator >= _model.Delay + _randomAdditionalDelay)
        {
            // _accumulator = 0;
            _hasDelay = false;
        }
        else if (!_hasDelay && !Invincible && _accumulator >= 100f / _model.Rate)
        {
            _accumulator = 0;
            Fire();
        }
    }

    private void OnBecameVisible()
    {
        isVisible = true;
        if (_model.Name != "Submarine")
            Invincible = false;
    }

    private void Fire()
    {
        if (_model.Name == "Aircraft Carrier")
        {
            animator.SetTrigger(MilitaryPlaneTakeoff);
            return;
        }

        const float range = 0.85f;
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
                MainCamera.AudioSource.PlayOneShot(empHitClip, 0.9f);
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

    public void Explode(bool reward = true)
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
            if (!Game.IsSpecialWave && !isSpecialShip)
                GameObject.FindWithTag("camera_container").GetComponent<Animator>()
                    .SetTrigger(Animator.StringToHash("one_shake"));
            var money = (int)(_model.Money * (isSpecialShip ? 2f : 1f));
            var floatingTextBig = Instantiate(this.floatingTextBig, GameObject.FindWithTag("canvas").transform);
            floatingTextBig.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+ {money} $";
            floatingTextBig.transform.position = transform.position + Vector3.up * 0.5f;
            Game.Money += money;
            Game.Score += _model.Health;
            _moneyCounter.UpdateUI();
            _scoreCounter.UpdateUI();
            if (isSpecialShip)
                GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().BeginSpecialWave();
            if (!Game.HasPowerUp && Random.Range(0f, 1f) < 0.18f)
            {
                var bound = new Bounds(
                    Vector3.zero,
                    new Vector2(MainCamera.MainCam.GetWidth(), MainCamera.MainCam.GetHeight()) * 2f
                );
                Instantiate(bubble).transform.position = transform.position + new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    0
                ); //bound.GetRandomPointInBounds();
            }
        }

        IEnumerator ScheduleDestroy()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

        StartCoroutine(ScheduleDestroy());
    }

    public void End() => Destroy(gameObject);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("base") && !_withBaseCollided)
        {
            _withBaseCollided = true;
            GameObject.FindWithTag("base").GetComponent<Base>().TakeDamage(_model.Damage);
            Explode(false);
        }
        else if (CurrentIndex != -1 && other.CompareTag("ship"))
        {
            var otherShip = other.GetComponent<Ship>();
            if (Invincible || otherShip.Invincible)
                return;
            if (!Collisions.Exists(it => it.Contains(CurrentIndex) && it.Contains(otherShip.CurrentIndex)))
            {
                // Handling collision with another ship
                Collisions.Add(new[] { CurrentIndex, otherShip.CurrentIndex });

                // Determine who should stop
                var bow = transform.Find("bow").position * spriteRenderer.bounds.size.x;
                if (Physics2D.OverlapCircleAll(bow, 0.5f).ToList().Exists(it => it.gameObject == other.gameObject))
                    GetComponent<ShipPath>().Wait = true;
                else
                    other.GetComponent<ShipPath>().Wait = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.Find("bow").position * spriteRenderer.bounds.size.x, 0.5f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (CurrentIndex != -1 && other.CompareTag("ship"))
        {
            var otherShip = other.GetComponent<Ship>();
            Collisions.RemoveAll(it => it.Contains(CurrentIndex) && it.Contains(otherShip.CurrentIndex));
            GetComponent<ShipPath>().Wait = false;
            other.gameObject.GetComponent<ShipPath>().Wait = false;
        }
    }

    public void SpawnMilitaryPlane()
    {
        Instantiate(militaryPlane);
    }
}