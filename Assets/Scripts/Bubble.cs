using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Interfaces;
using Managers;
using TMPro;
using UnityEngine;

public class Bubble : MonoBehaviour, IDamageble
{
    private static GameManager Game => GameManager.Instance;

    public enum PowerUp
    {
        Satellite,
        Damage,
        Rate,
        Speed,
        Health
    }

    private PowerUp _powerUp;
    private Animator _bubbleAnimator;
    [SerializeField] private Animator containerAnimator, smallBubblesAnimator;
    [SerializeField] private float lifespan = 7;
    [SerializeField] private List<GameObject> powerUps;
    [SerializeField] private GameObject text2X, floatingTextBig;
    [SerializeField] private AudioClip spawnAudioClip;
    private float _health;
    private MoneyCounter _moneyCounter, _scoreCounter;
    private float _acc;
    private bool _idling, _picking;
    private Transform _parent;
    private AudioSource _audioSource;
    private PowerUpsController _powerUpsController;
    private string _healthRestoreAudioClip = "Audio/health_restore";
    private WaveSpawner _waveSpawner;


    private void Awake()
    {
        _powerUp = Random.Range(0f, 1f) < 0.2f ? PowerUp.Satellite : EnumExtensions.RandomItem<PowerUp>();
        _health = (int)(120 * (1f + 4f * Game.WaveFactor));
        _bubbleAnimator = GetComponent<Animator>();
        _parent = transform.parent;
        _parent.GetComponent<Destroyable>().Condition = false;
        _audioSource = GetComponent<AudioSource>();
        _powerUpsController = GameObject.FindWithTag("power_ups_controller").GetComponent<PowerUpsController>();
        _moneyCounter = GameObject.FindWithTag("money_counter").GetComponent<MoneyCounter>();
        _scoreCounter = GameObject.FindWithTag("score_counter").GetComponent<MoneyCounter>();
        _waveSpawner = GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>();

        // Enable the correct powerUp and 2X accordingly
        powerUps.ForEach(it => it.SetActive(powerUps.IndexOf(it) == (int)_powerUp));
        text2X.SetActive((int)_powerUp > 0);
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
        if (_acc > lifespan / 2)
        {
            if (!_idling)
                containerAnimator.SetTrigger(Animator.StringToHash("fade"));
            _idling = true;

            // Increase fade speed with time
            containerAnimator.SetFloat(
                Animator.StringToHash("fade_speed"),
                1f + 2f * ((_acc - lifespan / 2f) / (lifespan / 2f)) // [1f, 3f]
            );
        }

        // Not picked up in time
        if (_acc >= lifespan)
            transform.parent.GetComponent<Destroyable>().Condition = true;
    }

    public void TakeDamage(int damage, bool _ = false)
    {
        if (_health > 0)
        {
            _health -= damage;
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
            if (_powerUp == PowerUp.Health)
            {
                _audioSource.PlayOneShot(Resources.Load(_healthRestoreAudioClip) as AudioClip);
                Game.Health = Mathf.Min(Game.MaxHealth, Game.Health + Game.MaxHealth / 3);
                GameObject.FindWithTag("base").GetComponent<Base>().UpdateUI();
                GameObject.FindWithTag("restore_health_hud").GetComponent<Animator>()
                    .SetTrigger(Animator.StringToHash("start"));
            }
            else if (!Game.HasPowerUp)
            {
                Game.PowerUp = _powerUp;
                if (_powerUp == PowerUp.Satellite)
                    _powerUpsController.PerformAttack();
                _powerUpsController.ShowRadialSlider();
            }
            else
            {
                var money = (int)Random.Range(20, 200 + 400 * Game.WaveFactor);
                var floatingTextBig = Instantiate(this.floatingTextBig, GameObject.FindWithTag("canvas").transform);
                floatingTextBig.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+ {money} $";
                floatingTextBig.transform.position = transform.position + Vector3.up * 0.5f;
                Game.Money += (int)(money);
                Game.Score += Game.Money * 2;
                _moneyCounter.UpdateUI();
                _scoreCounter.UpdateUI();
            }
        }
    }
}