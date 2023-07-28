using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Managers;
using Model;
using TMPro;
using UnityEngine;

public class Base : MonoBehaviour
{
    private static readonly int DamageHeavy = Animator.StringToHash("damage_heavy");
    private static GameManager Game => GameManager.Instance;
    [SerializeField] private GameObject cannon;
    [SerializeField] private GameObject turret;
    [SerializeField] private List<Animator> damageAnimators;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private AudioClip gameOver;

    void Update()
    {
        transform.rotation = MainCamera.MainCam.AngleToMouse(transform.position);
    }

    public void TakeDamage(int damage)
    {
        if (Game.Health > 0)
        {
            Game.Health -= damage;
            damageAnimators.ForEach(animator => animator.SetTrigger(DamageHeavy));
            healthBar.gameObject.SetActive(true);
            healthBar.SetValue(Game.Health / (float)Game.MaxHealth);
            if (Game.Health <= 0)
                StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);
        MainCamera.AudioSource.PlayOneShot(gameOver);

        // Disable ships
        foreach (var ship in GameObject.FindGameObjectsWithTag("ship"))
            ship.SetActive(false);

        // Show GameOver Menu
        var gameOverMenu = GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().gameOver;
        gameOverMenu.transform.Find("score_text").GetComponent<TextMeshProUGUI>().text =
            Game.Score.ToString("N0") + " pts";
        gameOverMenu.SetActive(true);
    }
}