using System;
using Managers;
using TMPro;
using UnityEngine;

public class AmmoCounter : MonoBehaviour
{
    private static PlayerManager Player => PlayerManager.Instance;
    [SerializeField] private TextMeshProUGUI textMeshPro;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI() =>
        textMeshPro.text = Player.Ammo + "/" + Player.MaxAmmo;
}