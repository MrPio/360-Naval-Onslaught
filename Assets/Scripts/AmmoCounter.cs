using System;
using Managers;
using Model;
using TMPro;
using UnityEngine;

public class AmmoCounter : MonoBehaviour
{
    private static PlayerManager Player => PlayerManager.Instance;
    private static TurretModel TurretModel => Player.CurrentTurret;

    [SerializeField] private TextMeshProUGUI textMeshPro;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI() =>
        textMeshPro.text = Player.Ammo + "/" + TurretModel.Ammo;
}