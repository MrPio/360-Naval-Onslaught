using System;
using System.Collections.Generic;
using ExtensionsFunctions;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    private static DataManager Data => DataManager.Instance;

    [SerializeField] private TextMeshProUGUI moneyText,
        waveText,
        hpText,
        criticalFactorText,
        turretCriticalChanceText,
        cannonCriticalChanceText,
        criticalFactorLevelText,
        turretCriticalChanceLevelText,
        cannonCriticalChanceLevelText;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image[] cannonSlots, turretSlots;
    [SerializeField] private GameObject[] cannonsLocks, turretsLocks;
    [SerializeField] private Sprite slotOn, slotOff;

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Header
        moneyText.text = Game.Money.ToString("N0");
        waveText.text = (Game.Wave).ToString();
        
        // Artillery tab
        healthSlider.maxValue = Game.MaxHealth;
        healthSlider.value = Game.Health;
        hpText.text = $"{Game.Health} / {Game.MaxHealth}";
        for (var i = 0; i < cannonsLocks.Length; i++)
            cannonsLocks[i].SetActive(Data.Cannons[i].IsLocked);
        for (var i = 0; i < turretsLocks.Length; i++)
            turretsLocks[i].SetActive(Data.Turrets[i].IsLocked);
        for (var i = 0; i < cannonSlots.Length; i++)
            cannonSlots[i].sprite = Game.CurrentCannon == i ? slotOn : slotOff;
        for (var i = 0; i < turretSlots.Length; i++)
            turretSlots[i].sprite = Game.CurrentTurret == i ? slotOn : slotOff;
        
        // CriticalHit tab
        criticalFactorText.text = $"+{(int)((Game.CriticalFactor - 1f) * 100f)}%";
        turretCriticalChanceText.text = $"{Math.Round(Game.TurretCriticalChance * 100f, 1)}%";
        cannonCriticalChanceText.text = $"{Math.Round(Game.CannonCriticalChance * 100f, 1)}%";
        criticalFactorLevelText.text = $"{Game.CriticalFactorLevel + 1}";
        turretCriticalChanceLevelText.text = $"{Game.TurretCriticalChanceLevel + 1}";
        cannonCriticalChanceLevelText.text = $"{Game.CannonCriticalChanceLevel + 1}";

        // PowerUp tab
        foreach (var powerUp in Data.PowerUps)
        {
            transform.Find(powerUp.Name + "_level").GetComponent<TextMeshProUGUI>().text =
                "lv. " + (powerUp.Level + 1);
            transform.Find(powerUp.Name + "_strength").GetComponent<TextMeshProUGUI>().text =
                (powerUp.Strength).ToString("N2") + "x";
            transform.Find(powerUp.Name + "_duration").GetComponent<TextMeshProUGUI>().text =
                (powerUp.Duration + 1).ToString("N1") + "s";
        }
    }
}