using System.Collections.Generic;
using ExtensionsFunctions;
using Managers;
using Model;
using UnityEngine;

public class Base : MonoBehaviour
{
    private static readonly int DamageHeavy = Animator.StringToHash("damage_heavy");
    private static GameManager Game => GameManager.Instance;
    [SerializeField] private GameObject cannon;
    [SerializeField] private GameObject turret;
    [SerializeField] private List<Animator> damageAnimators;
    [SerializeField] private HealthBar healthBar;

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
                GameOver();
        }
    }

    public void GameOver()
    {
    }
}