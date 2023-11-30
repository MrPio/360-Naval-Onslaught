using Managers;
using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
    private enum CounterType
    {
        Money,
        Score,
        Diamonds
    }

    private static GameManager Game => GameManager.Instance;

    private int Model => type switch
    {
        CounterType.Money => Game.Money,
        CounterType.Score => Game.Score,
        CounterType.Diamonds => Game.Diamonds
    };


    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Animator animator;
    private static readonly int Bounce = Animator.StringToHash("bounce");
    private int _lastMoney;
    private float _accumulator;
    private bool _animate;
    [SerializeField] private CounterType type = CounterType.Money;
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
        var currentMoney = (int)(_lastMoney + (Model - _lastMoney) * _accumulator / animationDuration);
        textMeshPro.text = currentMoney.ToString("N0");

        if (currentMoney >= Model)
        {
            _lastMoney = Model;
            textMeshPro.text = Model.ToString("N0");
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