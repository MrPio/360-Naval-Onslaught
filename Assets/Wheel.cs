using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Managers;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Wheel : MonoBehaviour
{
    private static DataManager Data => DataManager.Instance;
    private static GameManager Game => GameManager.Instance;

    [SerializeField] private GameObject wheel, tapText, floatingTextBig;
    [SerializeField] private float duration = 5f;
    [MinMaxSlider(0, 50)] [SerializeField] private Vector2 lapsRange;
    [SerializeField] private AudioClip spinningClip, moneyClip, diamondClip, scoreClip, menuIn, menuOut;
    [SerializeField] private AnimationCurve dragCurve;
    [SerializeField] private Animator animator;
    private Counter _scoreCounter, _moneyCounter, _diamondCounter;
    private int _chosenPrize;
    private float _totalAngle, _acc;
    private List<Transform> _slots;
    private bool _isSpinning;
    private float AngleStep => 360f / _slots.Count;
    private PrizeModel[] _prizes;

    private void Awake()
    {
        _slots = transform.GetComponentsInChildren<Transform>().Where(it => it.name is "slot").ToList();
        _prizes = new PrizeModel[_slots.Count];
        Data.SlotIndices.ForEach((slot, indices) =>
        {
            var slotsPrizes = Data.Prizes[slot].RandomSublist(indices.Count);
            indices.ForEach((index, i) =>
            {
                var prize = slotsPrizes[i];
                _prizes[index] = prize;
                _slots[index].GetComponentInChildren<TextMeshProUGUI>().text = prize.AmountText;
                _slots[index].GetComponent<Image>().sprite =
                    Resources.Load<Sprite>(PrizeModel.SlotSprites[prize.Type]);
            });
        });
        _scoreCounter = GameObject.FindWithTag("score_counter").GetComponent<Counter>();
        _moneyCounter = GameObject.FindWithTag("money_counter").GetComponent<Counter>();
        _diamondCounter = GameObject.FindWithTag("diamond_counter").GetComponent<Counter>();
    }

    private void Update()
    {
        if (!_isSpinning)
            return;
        _acc += Time.deltaTime;
        var progress = _acc / duration;
        if (progress >= 1)
        {
            _isSpinning = false;
            TakeReward();
        }
        else
            wheel.transform.localRotation =
                Quaternion.Euler(0, 0, Mathf.Lerp(0, _totalAngle, dragCurve.Evaluate(progress)));
    }

    private void OnEnable()
    {
        animator.SetTrigger(Animator.StringToHash("enter"));
        MainCamera.AudioSource.PlayOneShot(menuIn);
    }

    private void ShowText() =>
        tapText.SetActive(true);

    public void OnClick()
    {
        if (_isSpinning)
            return;
        tapText.SetActive(false);
        _chosenPrize = _prizes.ToList().IndexOf(_prizes.Where(it => !it.IsImpossible).ToList().RandomItem());
        var laps = (int)Random.Range(lapsRange.x, lapsRange.y);
        _totalAngle = 360f * laps + _chosenPrize * AngleStep + AngleStep * Random.Range(0.1f, 0.49f);
        _acc = 0;
        _isSpinning = true;
        MainCamera.AudioSource.PlayOneShot(spinningClip);
    }

    private void TakeReward()
    {
        var prize = _prizes[_chosenPrize];
        if (prize.Type == PrizeModel.PrizeType.Score)
        {
            MainCamera.AudioSource.PlayOneShot(scoreClip);
            Game.Score += prize.Amount;
            _scoreCounter.UpdateUI();
        }

        if (prize.Type == PrizeModel.PrizeType.Money)
        {
            MainCamera.AudioSource.PlayOneShot(moneyClip);
            Game.Money += prize.Amount;
            _moneyCounter.UpdateUI();
        }

        if (prize.Type == PrizeModel.PrizeType.Diamond)
        {
            MainCamera.AudioSource.PlayOneShot(diamondClip);
            Game.Diamonds += prize.Amount;
            Game.TotalDiamonds += prize.Amount;
            _diamondCounter.UpdateUI();
        }

        Instantiate(floatingTextBig, GameObject.FindWithTag("canvas").transform).Apply(it =>
        {
            it.transform.Find("Text").GetComponent<TextMeshProUGUI>().Apply(it =>
            {
                it.text = $"+ {prize.Amount}";
                it.color = new Color(48, 48, 48);
                it.fontSize *= 2f;
            });
            it.transform.position = Vector2.zero;
        });
        animator.SetTrigger(Animator.StringToHash("exit"));
    }

    private void Exit() => MainCamera.AudioSource.PlayOneShot(menuOut);
}