using System;
using System.Collections.Generic;
using ExtensionsFunctions;
using Interfaces;
using Managers;
using Model;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    private static InputManager In => InputManager.Instance;

    private TurretModel Model => GameManager.Instance.CurrentTurretModel;
    private float _accumulator;
    private List<IDamageable> _strickenDamagebles = new();
    private AmmoCounter _ammoCounter;
    [NonSerialized] public Transform Arm = null, Turret = null;
    [NonSerialized] public bool IsFreezed = false;

    private void Start()
    {
        _ammoCounter = GameObject.FindWithTag("ammo_counter").GetComponent<AmmoCounter>();
    }

    private void Update()
    {
        if (IsFreezed)
            return;
        if (Arm is { })
            transform.SetPositionAndRotation(Arm.position, In.GetInput().ToQuaternion());

        if (InputManager.Instance.GetTurretUp() || Game.Ammo <= 0)
        {
            if (Turret is { })
            {
                // Reload if no more ammo
                var turret = Turret.GetComponent<Turret>();
                if (!turret.reloadBar.IsReloading && Game.Ammo <= 0)
                    turret.Reload();
            }

            Destroy(gameObject);
            return;
        }

        _accumulator += Time.deltaTime;
        if (_accumulator >= 100f / Model.Rate)
        {
            _accumulator = 0;
            --Game.Ammo;
            _ammoCounter.UpdateUI();

            foreach (var strickenDamageable in _strickenDamagebles)
                strickenDamageable.TakeDamage(Model.Damage / 2, critical: Game.IsTurretCritical);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (IsFreezed)
            return;
        if ((col.gameObject.tag.Contains("ship") && !col.GetComponent<Ship>().Invincible) ||
            col.gameObject.tag.Contains("power_up"))
        {
            var go = col.gameObject.GetComponent<IDamageable>();
            if (!_strickenDamagebles.Contains(go))
                _strickenDamagebles.Add(go);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (IsFreezed)
            return;
        if ((col.gameObject.tag.Contains("ship") && !col.GetComponent<Ship>().Invincible) ||
            col.gameObject.tag.Contains("power_up"))
        {
            var go = col.gameObject.GetComponent<IDamageable>();
            if (_strickenDamagebles.Contains(go))
                _strickenDamagebles.Remove(go);
        }
    }
}