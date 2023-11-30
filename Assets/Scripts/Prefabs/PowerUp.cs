using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Model;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUp : MonoBehaviour, IDamageable
{
    private static GameManager Game => GameManager.Instance;

    private PowerUpModel _model;
    private Animator _bubbleAnimator;
    [SerializeField] private Animator containerAnimator, smallBubblesAnimator;
    [SerializeField] private List<GameObject> powerUps;
    [SerializeField] private GameObject floatingTextBig;
    [SerializeField] private TextMeshProUGUI text2X;
    [SerializeField] private Canvas canvas;
    [SerializeField] private AudioClip spawnAudioClip;
    private float _health;
    private Counter _counter, _scoreCounter;
    private float _acc;
    private bool _idling, _picking;
    private Transform _parent;
    private AudioSource _audioSource;
    private PowerUpsController _powerUpsController;
    private const string HealthRestoreAudioClip = "Audio/health_restore";
    private WaveSpawner _waveSpawner;


    private void Awake()
    {
        _model = Game.DrawPowerUp();
        if (_model is null)
        {
            Destroy(gameObject);
            return;
        }

        _health = _model.Health;
        canvas.worldCamera = MainCamera.MainCam;
        _bubbleAnimator = GetComponent<Animator>();
        _parent = transform.parent;
        _parent.GetComponent<Destroyable>().Condition = false;
        _audioSource = GetComponent<AudioSource>();
        _powerUpsController = GameObject.FindWithTag("power_ups_controller").GetComponent<PowerUpsController>();
        _counter = GameObject.FindWithTag("money_counter").GetComponent<Counter>();
        _scoreCounter = GameObject.FindWithTag("score_counter").GetComponent<Counter>();
        _waveSpawner = GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>();

        // Enable the correct powerUp and 2X accordingly
        powerUps.ForEach(it => it.SetActive(it.name.ToLower().Contains(_model.Name)));
        text2X.text = Math.Round(_model.Strength, 2) + "x";
        text2X.gameObject.SetActive(_model.IsMultiplier);
    }

    private void Start()
    {
        _audioSource.PlayOneShot(spawnAudioClip);
    }

    private void Update()
    {
        if (_picking)
            return;
        if (!_waveSpawner.isPaused)
            _acc += Time.deltaTime;

        // Fading animation
        if (_acc > _model.Lifespan / 2)
        {
            if (!_idling)
                containerAnimator.SetTrigger(Animator.StringToHash("fade"));
            _idling = true;

            // Increase fade speed with time
            containerAnimator.SetFloat(
                Animator.StringToHash("fade_speed"),
                1f + 2f * ((_acc - _model.Lifespan / 2f) / (_model.Lifespan / 2f)) // [1f, 3f]
            );
        }

        // Not picked up in time
        if (_acc >= _model.Lifespan)
            transform.parent.GetComponent<Destroyable>().Condition = true;
    }

    public void TakeDamage(int damage, bool critical = false, bool _ = false)
    {
        if (_health > 0)
        {
            if (critical)
                damage = (int)(damage * Game.CriticalFactor);
            _health -= damage;
            GetComponent<Damageable>()?.Damage(damage, critical: critical);
            if (_health <= 0)
                Explode();
            else
                _bubbleAnimator.SetTrigger(Animator.StringToHash("damage"));
        }
    }

    public void Explode(bool reward = true)
    {
        _picking = true;
        var pop = Animator.StringToHash("pop");
        containerAnimator.SetTrigger(pop);
        _bubbleAnimator.SetTrigger(pop);
        smallBubblesAnimator.SetTrigger(pop);
        transform.parent.GetComponent<Destroyable>().Condition = true;
        if (reward)
        {
            _audioSource.Play();
            // Instantaneous PowerUp
            if (_model.Type == PowerUpModel.PowerUp.Health)
            {
                _audioSource.PlayOneShot(Resources.Load(HealthRestoreAudioClip) as AudioClip);
                Game.Health = Mathf.Min(
                    Game.MaxHealth,
                    Game.Health + (int)(Game.MaxHealth * _model.Strength / 4f)
                );
                GameObject.FindWithTag("base").GetComponent<Base>().UpdateUI();
                GameObject.FindWithTag("restore_health_hud").GetComponent<Animator>()
                    .SetTrigger(Animator.StringToHash("start"));
            }
            // Durable PowerUp
            else if (!Game.HasPowerUp)
            {
                Game.PowerUp = _model;
                if (_model.Type == PowerUpModel.PowerUp.Satellite)
                    _powerUpsController.PerformAttack(_model);
                _powerUpsController.ShowRadialSlider();
            }
            // Money reward if already using a durable PowerUp
            else
            {
                var money = (int)Random.Range(20, 200 + 400 * Game.WaveFactor);
                var floatingTextBig = Instantiate(this.floatingTextBig, GameObject.FindWithTag("canvas").transform);
                floatingTextBig.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+ {money} $";
                floatingTextBig.transform.position = transform.position + Vector3.up * 0.5f;
                Game.Money += money;
                Game.Score += money * 2;
                _counter.UpdateUI();
                _scoreCounter.UpdateUI();
            }
        }
    }
}