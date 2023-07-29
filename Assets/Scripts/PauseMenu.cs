using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameTimeText, waveText, shipDestroyedText;

    private void OnEnable()
    {
        gameTimeText.text = (Time.realtimeSinceStartup - WaveSpawner.GameStarted).ToString("N1") + "sec";
        waveText.text = (GameManager.Instance.Wave + 1).ToString();
        shipDestroyedText.text = DataManager.Instance.Waves.Sum(it => it.Destroyed).ToString();
    }
}