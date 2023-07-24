using System;
using Managers;
using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Animator animator;
    private static readonly int Bounce = Animator.StringToHash("bounce");
    private int _lastMoney;
    private float _accumulator;
    private bool _animate;
    [SerializeField] private float animationDuration = 2;


    private void Start()
    {
        _lastMoney = 0;
        UpdateUI();
    }

    private void Update()
    {
        if (!_animate) return;

        _accumulator += Time.deltaTime;
        var currentMoney = (int)(_lastMoney + (Game.Money - _lastMoney) * _accumulator / animationDuration);
        textMeshPro.text = currentMoney.ToString("N0");

        if (currentMoney >= Game.Money)
        {
            _lastMoney = Game.Money;
            textMeshPro.text = Game.Money.ToString("N0");
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