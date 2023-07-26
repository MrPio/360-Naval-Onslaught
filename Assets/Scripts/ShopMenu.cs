using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText, waveText, hpText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private SpriteRenderer[] cannonSlots,turretSlots;
    [SerializeField] private GameObject[] cannonsLocks,turretsLocks;
}
