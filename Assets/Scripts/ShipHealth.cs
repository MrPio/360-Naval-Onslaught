using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHealth : MonoBehaviour
{
    private static readonly int ShipDamage = Animator.StringToHash("ship_damage");
    private static readonly int ShipDestroy = Animator.StringToHash("ship_destroy");
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private int maxHealth;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Animator animator;
    private int health;

    void Start()
    {
        health = maxHealth;
        healthBar.gameObject.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        animator.SetTrigger(ShipDamage);
        healthBar.gameObject.SetActive(true);
        healthBar.setValue(health / (float)maxHealth);
        if (health <= 0)
            Explode();
    }

    private void Explode()
    {
        animator.SetTrigger(ShipDestroy);
        Instantiate(explosion, transform);
    }

    public void End() => Destroy(gameObject);
}