using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    private static DataManager Data => DataManager.Instance;
    [SerializeField] private TextMeshProUGUI moneyText, waveText, hpText;
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
        moneyText.text = Game.Money.ToString("N0");
        waveText.text = (Game.Wave + 1).ToString();
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
    }
}