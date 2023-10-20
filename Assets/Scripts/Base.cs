using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Base : MonoBehaviour
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

    private void Start()
    {
        shield.SetActive(false);
    }

    private void Update()
    {
        // transform.rotation = In.GetInput().ToQuaternion();

        for (var i = 0; i < 4; i++)
        {
            if (In.SpecialDown(i) && !_lastSpecialInput[i])
                UseSpecial(i);
            _lastSpecialInput[i] = In.SpecialDown(i);
        }

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            pauseMenu.SetActive(true);
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().isPaused = true;
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
        healthBar.SetValue(Game.Health / (float)Game.MaxHealth);
        shield.SetActive(false);
        _invincible = false;
    }

    public void TakeDamage(int damage)
    {
        if (Game.Health > 0 && !_invincible)
        {
            StartCoroutine(In.Vibrate());
            Game.Health -= damage;
            var fraction = Game.Health / (float)Game.MaxHealth;
            damageAnimators.ForEach(animator => animator.SetTrigger(DamageHeavy));
            chromaticAberration.SetTrigger(Start1);
            GetComponent<GlitchEffect>().Animate();
            if (!Game.IsSpecialWave)
                cameraContainerAnimator.SetTrigger("one_shake");
            healthBar.gameObject.SetActive(true);
            healthBar.SetValue(fraction);
            lowHealthHUD.Evaluate(fraction);
            if (Game.Health <= 0)
                StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);

        // Play game over clip on WaveSpawner's audioSource
        var audioSource = GameObject.FindWithTag("wave_spawner").GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = gameOverClip;
        audioSource.Play();

        // Disable ships
        foreach (var ship in GameObject.FindGameObjectsWithTag("ship"))
            ship.SetActive(false);

        // Show GameOver Menu
        var gameOverMenu = GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().gameOver;
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
            healthBar.SetValue(1);
        }

        --Game.SpecialsCount[index];
        specialsCounter.UpdateUI();
        _lastSpecialUsed = Time.time;
    }
}