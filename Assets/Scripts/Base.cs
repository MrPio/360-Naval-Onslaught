using System.Collections.Generic;
using ExtensionsFunctions;
using Managers;
using UnityEngine;

public class Base : MonoBehaviour
{
    private static readonly int DamageHeavy = Animator.StringToHash("damage_heavy");
    private static GameManager Game => GameManager.Instance;
    [SerializeField] private GameObject cannon;
    [SerializeField] private GameObject turret;
    [SerializeField] private List<Animator> damageAnimators;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private ReloadBar reloadBar;
    [SerializeField] private AudioClip reloadStart,reloadMiss;

    void Update()
    {
        transform.rotation = MainCamera.mainCam.AngleToMouse(transform.position);
        if (!reloadBar.IsReloading && Input.GetKeyDown(KeyCode.R))
        {
            if (Game.Ammo < Game.CurrentTurretModel.Ammo)
            {
                MainCamera.AudioSource.PlayOneShot(reloadStart);
                reloadBar.Reload();
            }
            else
            {
                MainCamera.AudioSource.PlayOneShot(reloadMiss);
            }
        }
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
                GameOver();
        }
    }

    private void GameOver()
    {
    }
}