using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusMenu : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;

    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private Image specialImage;
    private int _index;
    private float _accumulator;
    [SerializeField] private List<Sprite> specialSprites;
    [SerializeField] private float step = 0.2f;
    [NonSerialized] public bool Stop;
    [SerializeField] private SpecialsCounter specialsCounter;

    private void OnEnable()
    {
        Stop = false;
        _accumulator = 0;
        _index = 0;
    }

    private void Update()
    {
        if (!Stop)
            _accumulator += Time.deltaTime;
        if (_accumulator >= step)
        {
            _accumulator = 0;
            ++_index;
            subtitleText.text = Game.SpecialsName[_index % Game.SpecialsName.Count] + " +1";
            specialImage.sprite = specialSprites[_index % Game.SpecialsName.Count];
        }
    }

    public void Redeem()
    {
        ++Game.SpecialsCount[_index % Game.SpecialsName.Count];
        specialsCounter.UpdateUI();
        
    }
}