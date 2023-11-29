using System;
using System.Linq;
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
        cannonCriticalChanceLevelText,
        powerUpSpawnChance,
        powerUpSpawnChanceLevel;

    [SerializeField] private TextMeshProUGUI[] powerUpLevels, powerUpStrengths, powerUpDurations;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image[] cannonSlots, turretSlots, powerUpSlots;
    [SerializeField] private GameObject[] cannonsLocks, turretsLocks, powerUpLocks;
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
        powerUpSpawnChance.text = $"{Math.Round(Game.PowerUpSpawnChance * 100f, 1)}%";
        powerUpSpawnChanceLevel.text = $"{Game.PowerUpSpawnChanceLevel + 1}";
        foreach (var powerUp in Data.PowerUps)
        {
            powerUpLevels[powerUp.Index].text = powerUp.IsLocked ? "-" : "lv. " + (powerUp.Level + 1);
            powerUpStrengths[powerUp.Index].text = powerUp.IsLocked ? "-" : powerUp.Strength.ToString("N2") + "x";
            powerUpDurations[powerUp.Index].text = (powerUp.IsLocked || !powerUp.HasDuration)
                ? "-"
                : powerUp.Duration.ToString("N0") + "s";
        }

        for (var i = 0; i < powerUpSlots.Length; i++)
            powerUpSlots[i].color = Color.white.WithAlpha(Data.PowerUps[i].IsLocked ? 0.5f : 1f);
        for (var i = 0; i < powerUpLocks.Length; i++)
            powerUpLocks[i].SetActive(Data.PowerUps[i].IsLocked);
    }
}