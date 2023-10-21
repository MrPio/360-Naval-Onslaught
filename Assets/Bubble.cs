using System;
using System.Collections.Generic;
using ExtensionsFunctions;
using Interfaces;
using Managers;
using Mono.Cecil;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private PowerUp _powerUp = EnumExtensions.RandomItem<PowerUp>();
    private Animator _bubbleAnimator;
    [SerializeField] private Animator containerAnimator, smallBubblesAnimator;
    [SerializeField] private float health = 120, lifespan = 7;
    [SerializeField] private List<GameObject> powerUps;
    [SerializeField] private GameObject text2X;
    [SerializeField] private AudioClip spawnAudioClip;
    private float _acc;
    private bool _idling, _picking;
    private Transform _parent;
    private AudioSource _audioSource;
    private string _healthRestoreAudioClip = "Audio/health_restore";


    private void Awake()
    {
        _bubbleAnimator = GetComponent<Animator>();
        _parent = transform.parent;
        _parent.GetComponent<Destroyable>().Condition = false;
        _audioSource = GetComponent<AudioSource>();

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
        if (health > 0)
        {
            health -= damage;
            if (health <= 0)
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
            Game.PowerUp = _powerUp;
            if (_powerUp == PowerUp.Health)
            {
                _audioSource.PlayOneShot(Resources.Load(_healthRestoreAudioClip) as AudioClip);
                Game.Health = Mathf.Min(Game.MaxHealth, Game.Health + Game.MaxHealth / 3);
                GameObject.FindWithTag("base").GetComponent<Base>().UpdateUI();
                GameObject.FindWithTag("restore_health_hud").GetComponent<Animator>()
                    .SetTrigger(Animator.StringToHash("start"));
            }
        }
    }
}