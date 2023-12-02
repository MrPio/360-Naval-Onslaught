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
using Unity.Mathematics;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Ship : MonoBehaviour, IDamageable
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
    [SerializeField] private GameObject floatingTextBig, missile, empExplosion, empText, militaryPlane;
    [SerializeField] private GameObject powerUp, diamond;
    [SerializeField] private AudioClip empHitClip;
    private Sprite _missileSprite;
    private List<AudioClip> _fireClip;
    private Counter _counter, _scoreCounter;
    private AudioClip _explodeClip;
    private ShipModel _model;
    private int _health;
    private bool _hasDelay = true;
    [NonSerialized] public bool IsFreezed;
    private float _accumulator;
    [NonSerialized] public bool Invincible = true;
    private int _currentIndex = -1;
    private float _randomAdditionalDelay;
    private static readonly int MilitaryPlaneTakeoff = Animator.StringToHash("military_plane_takeoff");
    [NonSerialized] public bool isVisible;
    private bool _withBaseCollided;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip specialAudioClip, armoredSpawnClip, dropArmorPieceClip;
    [SerializeField] private GameObject glow, armor, armorPiece;
    [SerializeField] private List<GameObject> armorPieces;
    [SerializeField] private bool alwaysSpecial;
    private bool _isSpecial, _isArmored;
    private int _activeArmorPieces, _totalArmorPieces;
    private int MaxHealth => _model.Health * (_isArmored ? 4 : 1);

    private void Awake()
    {
        if (Game.IsSpecialWave)
            _model = DataManager.Instance.Ships[0];
        else
        {
            var pair = Game.CurrentWave.Spawn();
            _currentIndex = pair?.Key ?? 0;
            _model = pair?.Value;
        }

        if (_model.ExplodeClip != null)
            _explodeClip = Resources.Load<AudioClip>(_model.ExplodeClip);
        if (_model.FireClip != null)
            _fireClip = _model.FireClip.Select(Resources.Load<AudioClip>).ToList();
        _counter = GameObject.FindWithTag("money_counter").GetComponent<Counter>();
        _scoreCounter = GameObject.FindWithTag("score_counter").GetComponent<Counter>();
        GetComponent<ShipPath>().Model = _model;
        if (_model.MissileSprite is { })
            _missileSprite = Resources.Load<Sprite>(_model.MissileSprite);

        // Custom Path for SpeedBoat
        if (_model.Name == "SpeedBoat")
        {
            var specialBoat = !Game.IsSpecialWave && !Game.SpecialOccurInWave[Game.Wave] &&
                              (alwaysSpecial || Random.Range(0f, 1f) < Game.SpecialShipChance);
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
                GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().SpecialShipSpawned();
                audioSource.PlayOneShot(specialAudioClip);
                GetComponent<ShipPath>().SpeedMultiplier = ShipModel.SpecialSpeedMultiplier;
                animator.SetTrigger(Animator.StringToHash("special_ship"));
                glow.SetActive(true);
                _isSpecial = true;
            }
            else if (!Game.IsSpecialWave && Game.DrawArmoredShip)
            {
                _isArmored = true;
                armor.SetActive(true);
                _totalArmorPieces = armorPieces.Count / 3;
                _activeArmorPieces = _totalArmorPieces;
                GetComponent<ShipPath>().SpeedMultiplier = ShipModel.ArmoredSpeedMultiplier;
            }
        }

        spriteRenderer.sprite = Resources.Load<Sprite>(_model.Sprite + (_isArmored ? "_armored" : ""));

        _randomAdditionalDelay = Random.Range(0f, 1f);
        foamAnimator.Play(_model.FoamAnim);

        // Set Fog strength
        if (_currentIndex == Game.CurrentWave.startFog)
            GameObject.FindWithTag("cloud_manager").GetComponent<CloudManager>()
                .SetStrength(Random.Range(Game.CurrentWave.FogStrength * 0.75f,
                    Game.CurrentWave.FogStrength * 1.5f));
        if (_currentIndex == Game.CurrentWave.endFog)
            GameObject.FindWithTag("cloud_manager").GetComponent<CloudManager>().SetStrength();
    }

    private void Start()
    {
        _health = MaxHealth;
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
        if (_isArmored)
            MainCamera.AudioSource.PlayOneShot(armoredSpawnClip);
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

    public void TakeDamage(int damage, bool critical = false, bool emp = false)
    {
        if (!Invincible && _health > 0)
        {
            if (critical)
                damage = (int)(damage * Game.CriticalFactor);
            _health -= damage;
            _health = Mathf.Max(_health, 0);
            GetComponent<Damageable>()?.Damage(damage, critical: critical, armored: _isArmored);
            animator.SetTrigger(ShipDamage);
            healthBar.SetValue(_health / (float)MaxHealth);

            if (emp)
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

            if (_isArmored)
            {
                var armorLeft = (int)(_health / (float)MaxHealth * _totalArmorPieces);
                if (armorLeft < _activeArmorPieces && armorPieces.Count > 0)
                {
                    var diff = _activeArmorPieces - armorLeft;
                    _activeArmorPieces = armorLeft;
                    for (var i = 0; i < diff; i++)
                        armorPieces.RandomItem().Apply(it =>
                        {
                            DropArmorPiece(it);
                            armorPieces.Remove(it);
                        });
                }
            }

            if (_health <= 0)
                Explode();
        }
    }

    private void DropArmorPiece(GameObject piece)
    {
        Instantiate(armorPiece).transform.Apply(it =>
        {
            var localPosition = piece.transform.localPosition;
            var startPos = piece.transform.position + localPosition * 0.25f;
            it.position = startPos;
            it.GetComponent<ArmorPiece>()
                .Drop(startPos + localPosition * Random.Range(1.5f, 3.5f));
        });
        MainCamera.AudioSource.PlayOneShot(dropArmorPieceClip);
        // Destroy(piece);
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


        // Money Reward and power up spawn
        if (reward)
        {
            if (!Game.IsSpecialWave && !_isSpecial)
                GameObject.FindWithTag("camera_container").GetComponent<Animator>()
                    .SetTrigger(Animator.StringToHash("one_shake"));
            var money = (int)(_model.Money * (_isSpecial ? 2f : 1f) * (_isArmored ? 2f : 1f));
            var floatingTextBig = Instantiate(this.floatingTextBig, GameObject.FindWithTag("canvas").transform);
            floatingTextBig.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+ {money} $";
            floatingTextBig.transform.position = transform.position + Vector3.up * 0.5f;
            Game.Money += money;
            Game.Score += MaxHealth;
            _counter.UpdateUI();
            _scoreCounter.UpdateUI();
            if (_isSpecial)
                GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().BeginSpecialWave();
            if (_isArmored && Game.CanSpawnDiamond && Game.DrawDiamond)
            {
                Instantiate(diamond).transform.position = transform.position + new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    0
                );
                ++Game.PendingDiamonds;
            }

            if (!Game.HasPowerUp && Game.DrawPowerUpSpawn)
                Instantiate(powerUp).transform.position = transform.position + new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    0
                );
        }

        if (_isArmored)
            armorPieces.ForEach(DropArmorPiece);

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
            GameObject.FindWithTag("base").GetComponent<Base>().TakeDamage((int)(_model.Damage *
                (_isArmored ? 1.5f : 1f)));
            Explode(false);
        }
        else if (_currentIndex != -1 && other.CompareTag("ship"))
        {
            var otherShip = other.GetComponent<Ship>();
            if (Invincible || otherShip.Invincible)
                return;
            if (!Collisions.Exists(it => it.Contains(_currentIndex) && it.Contains(otherShip._currentIndex)))
            {
                // Handling collision with another ship
                Collisions.Add(new[] { _currentIndex, otherShip._currentIndex });

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
        if (_currentIndex != -1 && other.CompareTag("ship"))
        {
            var otherShip = other.GetComponent<Ship>();
            Collisions.RemoveAll(it => it.Contains(_currentIndex) && it.Contains(otherShip._currentIndex));
            GetComponent<ShipPath>().Wait = false;
            other.gameObject.GetComponent<ShipPath>().Wait = false;
        }
    }

    public void SpawnMilitaryPlane()
    {
        Instantiate(militaryPlane);
    }
}