using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Managers;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Base : MonoBehaviour,IDamageble
{
    private static readonly int DamageHeavy = Animator.StringToHash("damage_heavy");
    private static readonly int Start1 = Animator.StringToHash("start");
    private static GameManager Game => GameManager.Instance;
    private static InputManager In => InputManager.Instance;
    [SerializeField] private GameObject cannon, turret, allyPlane, shield;
    [SerializeField] private List<Animator> damageAnimators;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private AudioClip gameOverClip, noSpecialClip, shieldClip, empClip, heartClip;
    [SerializeField] private Animator chromaticAberration;
    [SerializeField] private SpecialsCounter specialsCounter;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private LowHealthHUD lowHealthHUD;
    [SerializeField] private Animator cameraContainerAnimator;
    private bool _invincible;
    private float _lastSpecialUsed;
    private bool[] _lastSpecialInput = { false, false, false, false };
    private WaveSpawner _waveSpawner;
    private void Start()
    {
        shield.SetActive(false);
        _waveSpawner = GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>();
    }

    private void Update()
    {
        // transform.rotation = In.GetInput().ToQuaternion();

        for (var i = 0; i < 4; i++)
        {
            var tmp = In.SpecialDown(i);
            if (tmp && !_lastSpecialInput[i])
                UseSpecial(i);
            _lastSpecialInput[i] = tmp;
        }

        if (In.GetPause() && !_waveSpawner.isPaused)
        {
            pauseMenu.SetActive(true);
            _waveSpawner.isPaused = true;
            foreach (var ship in GameObject.FindGameObjectsWithTag("ship"))
            {
                ship.GetComponent<Ship>().IsFreezed = true;
                ship.GetComponent<ShipPath>().IsFreezed = true;
            }

            foreach (var bullet in GameObject.FindGameObjectsWithTag("bullet"))
                bullet.GetComponent<Bullet>().IsFrozen = true;
            foreach (var laser in GameObject.FindGameObjectsWithTag("laser"))
                laser.GetComponent<Laser>().IsFreezed = true;

            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        UpdateUI();
        shield.SetActive(false);
        _invincible = false;
    }

    public void UpdateUI()
    {
        var fraction = Game.Health / (float)Game.MaxHealth;
        healthBar.SetValue(fraction);
        lowHealthHUD.Evaluate(fraction);
    }
    
    public void TakeDamage(int damage,bool _=false)
    {
        if (Game.Health > 0 && !_invincible)
        {
            StartCoroutine(In.Vibrate());
            Game.Health -= damage;
            damageAnimators.ForEach(animator => animator.SetTrigger(DamageHeavy));
            chromaticAberration.SetTrigger(Start1);
            GetComponent<GlitchEffect>().Animate();
            if (!Game.IsSpecialWave)
                cameraContainerAnimator.SetTrigger(Animator.StringToHash("one_shake"));
            healthBar.gameObject.SetActive(true);
            UpdateUI();
            if (Game.Health <= 0)
                StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);

        // Play game over clip on WaveSpawner's audioSource
        var audioSource = _waveSpawner.GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = gameOverClip;
        audioSource.Play();

        // Disable ships
        foreach (var ship in GameObject.FindGameObjectsWithTag("ship"))
            ship.SetActive(false);

        // Show GameOver Menu
        var gameOverMenu = _waveSpawner.gameOver;
        gameOverMenu.SetActive(true);
        gameOverMenu.transform.Find("score_text").GetComponent<TextMeshProUGUI>().text =
            Game.Score.ToString("N0") + " pts";
    }

    private void UseSpecial(int index)
    {
        if (Game.SpecialsCount[index] <= 0 || Time.time - _lastSpecialUsed < 1.8f
                                           || (index == 1 && _invincible)
                                           || (index == 3 && Game.Health >= Game.MaxHealth))
        {
            MainCamera.AudioSource.PlayOneShot(noSpecialClip);
            return;
        }

        if (index == 0)
        {
            IEnumerator SpawnPlanes()
            {
                for (var i = 0; i < 5; i++)
                {
                    Instantiate(allyPlane);
                    yield return new WaitForSeconds(2f * Random.Range(0.75f, 1.15f));
                }
            }

            StartCoroutine(SpawnPlanes());
        }
        else if (index == 1)
        {
            MainCamera.AudioSource.PlayOneShot(shieldClip);
            shield.SetActive(true);
            _invincible = true;

            IEnumerator EndInvincibility()
            {
                yield return new WaitForSeconds(20f);
                shield.SetActive(false);
                _invincible = false;
            }

            StartCoroutine(EndInvincibility());
        }
        else if (index == 2)
        {
            bool found = false;
            foreach (var ship in GameObject.FindGameObjectsWithTag("ship").Select(it => it.GetComponent<Ship>()))
                if (ship.isVisible)
                {
                    found = true;
                    ship.TakeDamage(0, true);
                }

            if (!found)
            {
                MainCamera.AudioSource.PlayOneShot(noSpecialClip);
                return;
            }
            else
            {
                MainCamera.AudioSource.PlayOneShot(empClip);
            }
        }
        else if (index == 3)
        {
            MainCamera.AudioSource.PlayOneShot(heartClip);
            Game.Health = Game.MaxHealth;
            UpdateUI();
        }

        --Game.SpecialsCount[index];
        specialsCounter.UpdateUI();
        _lastSpecialUsed = Time.time;
    }


    public void Explode(bool reward = true)
    {
        throw new NotImplementedException();
    }
}