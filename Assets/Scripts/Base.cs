using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Managers;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Base : MonoBehaviour
{
    private static readonly int DamageHeavy = Animator.StringToHash("damage_heavy");
    private static readonly int Start1 = Animator.StringToHash("start");
    private static GameManager Game => GameManager.Instance;
    [SerializeField] private GameObject cannon, turret, allyPlane, shield;
    [SerializeField] private List<Animator> damageAnimators;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private AudioClip gameOverClip, noSpecialClip, shieldClip, empClip, heartClip;
    [SerializeField] private Animator chromaticAberration;
    [SerializeField] private SpecialsCounter specialsCounter;
    [SerializeField] private GameObject pauseMenu;
    private bool _invincible;
    private float lastSpecialUsed;

    private void Start()
    {
        shield.SetActive(false);
    }

    private void Update()
    {
        transform.rotation = MainCamera.MainCam.AngleToMouse(transform.position);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            UseSpecial(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            UseSpecial(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            UseSpecial(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            UseSpecial(3);

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(true);
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().isPaused = true;
            foreach (var ship in GameObject.FindGameObjectsWithTag("ship"))
            {
                ship.GetComponent<Ship>().IsFreezed = true;
                ship.GetComponent<ShipPath>().IsFreezed = true;
            }
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        healthBar.SetValue(Game.Health / (float)Game.MaxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (Game.Health > 0 && !_invincible)
        {
            Game.Health -= damage;
            damageAnimators.ForEach(animator => animator.SetTrigger(DamageHeavy));
            chromaticAberration.SetTrigger(Start1);
            healthBar.gameObject.SetActive(true);
            healthBar.SetValue(Game.Health / (float)Game.MaxHealth);
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
        if (Game.SpecialsCount[index] <= 0 || Time.time - lastSpecialUsed < 1.8f
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
            bool found=false;
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
        lastSpecialUsed = Time.time;
    }
}