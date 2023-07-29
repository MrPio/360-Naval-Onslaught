using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialsCounter : MonoBehaviour
{
    private List<int> SpecialsCount => GameManager.Instance.SpecialsCount;
    [SerializeField] private List<TextMeshProUGUI> counters;

    public void UpdateUI()
    {
        for (var i = 0; i < SpecialsCount.Count; i++)
        {
            counters[i].text = SpecialsCount[i].ToString("N0");
            counters[i].color = SpecialsCount[i] > 0 ? Color.blue : Color.white;
        }
    }

    private void OnEnable()
    {
        UpdateUI();
    }
}