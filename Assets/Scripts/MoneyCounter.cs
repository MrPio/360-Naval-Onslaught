using System;
using Managers;
using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    private int _model=>isScore?Game.Score:Game.Money;
    
    
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Animator animator;
    private static readonly int Bounce = Animator.StringToHash("bounce");
    private int _lastMoney;
    private float _accumulator;
    private bool _animate;
    [SerializeField] public bool isScore = false;
    [SerializeField] private float animationDuration = 2;


    private void Start()
    {
        _lastMoney = 0;
        UpdateUI();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (!_animate) return;

        _accumulator += Time.deltaTime;
        var currentMoney = (int)(_lastMoney + (_model - _lastMoney) * _accumulator / animationDuration);
        textMeshPro.text = currentMoney.ToString("N0");

        if (currentMoney >= _model)
        {
            _lastMoney = _model;
            textMeshPro.text = _model.ToString("N0");
            _animate = false;
        }
    }

    public void UpdateUI()
    {
        animator.SetTrigger(Bounce);
        if (!_animate)
        {
            _animate = true;
            _accumulator = 0;
        }
    }
}