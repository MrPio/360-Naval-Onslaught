using System.Collections.Generic;
using ExtensionsFunctions;
using Managers;
using UnityEngine;

public class Base : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    [SerializeField] private GameObject cannon;
    [SerializeField] private GameObject turret;
    [SerializeField] private List<Animator> damageAnimators;
    [SerializeField] private HealthBar healthBar;
    private static readonly int ShipDamage = Animator.StringToHash("ship_damage");

    void Update()
    {
        transform.rotation = MainCamera.mainCam.AngleToMouse(transform.position);
    }

    public void TakeDamage(int damage)
    {
        if (Game.Health > 0)
        {
            Game.Health -= damage;
            damageAnimators.ForEach(animator=>animator.SetTrigger(ShipDamage));
            healthBar.gameObject.SetActive(true);
            healthBar.setValue(Game.Health / (float)Game.MaxHealth);
            if (Game.Health <= 0)
                GameOver();
        }
    }

    private void GameOver()
    {
        
    }
}