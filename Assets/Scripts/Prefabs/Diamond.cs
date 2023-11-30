using System;
using System.Collections.Generic;
using ExtensionsFunctions;
using Interfaces;
using Managers;
using Model;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Diamond : MonoBehaviour, IDamageable
{
    private static GameManager Game => GameManager.Instance;

    [SerializeField] private Animator containerAnimator;
    [SerializeField] private GameObject floatingTextBig;
    [SerializeField] private AudioClip spawnAudioClip, collectClip;
    private float _health;
    private Counter _scoreCounter, _diamondCounter;
    private float _acc;
    private bool _idling, _picking;
    private Transform _parent;
    private WaveSpawner _waveSpawner;


    private void Awake()
    {
        _health = Game.DiamondHealth;
        _parent = transform.parent;
        _parent.GetComponent<Destroyable>().Condition = false;
        _scoreCounter = GameObject.FindWithTag("score_counter").GetComponent<Counter>();
        _diamondCounter = GameObject.FindWithTag("diamond_counter").GetComponent<Counter>();
        _waveSpawner = GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>();
    }

    private void Start()
    {
        MainCamera.AudioSource.PlayOneShot(spawnAudioClip);
    }

    private void Update()
    {
        if (_picking)
            return;
        if (!_waveSpawner.isPaused)
            _acc += Time.deltaTime;

        // Fading animation
        if (_acc > Game.DiamondLifespan / 2)
        {
            if (!_idling)
                containerAnimator.SetTrigger(Animator.StringToHash("fade"));
            _idling = true;

            // Increase fade speed with time
            containerAnimator.SetFloat(
                Animator.StringToHash("fade_speed"),
                1f + 2f * ((_acc - Game.DiamondLifespan / 2f) / (Game.DiamondLifespan / 2f)) // [1f, 3f]
            );
        }

        // Not picked up in time
        if (_acc >= Game.DiamondLifespan)
        {
            --Game.PendingDiamonds;
            transform.parent.GetComponent<Destroyable>().Condition = true;
        }
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
                containerAnimator.SetTrigger(Animator.StringToHash("damage"));
        }
    }

    public void Explode(bool reward = true)
    {
        _picking = true;
        containerAnimator.SetTrigger(Animator.StringToHash("pop"));
        transform.parent.GetComponent<Destroyable>().Condition = true;
        if (reward)
        {
            MainCamera.AudioSource.PlayOneShot(collectClip);
            Instantiate(this.floatingTextBig, GameObject.FindWithTag("canvas").transform).Apply(it =>
            {
                it.transform.Find("Text").GetComponent<TextMeshProUGUI>().Apply(it =>
                {
                    it.text = $"+ 1";
                    it.color = new Color(0, 86, 255);
                });
                it.transform.position = transform.position + Vector3.up * 0.5f;
            });
            Game.Score += 500;
            ++Game.Diamonds;
            ++Game.TotalDiamonds;
            --Game.PendingDiamonds;
            _scoreCounter.UpdateUI();
            _diamondCounter.UpdateUI();
        }
    }
}