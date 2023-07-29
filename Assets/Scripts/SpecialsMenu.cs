using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;

public class SpecialsMenu : MonoBehaviour
{
    private List<int> SpecialsCount => GameManager.Instance.SpecialsCount;
    [SerializeField] private List<TextMeshProUGUI> counters;

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (var i = 0; i < SpecialsCount.Count; i++)
            counters[i].text = SpecialsCount[i].ToString("N0");
    }
}