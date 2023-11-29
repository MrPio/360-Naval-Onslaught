using System;
using Managers;
using Model;
using UnityEngine;
using UnityEngine.UI;

public class ReloadBar : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;

    [SerializeField] private Slider slider;
    [NonSerialized] public bool IsReloading;
    [NonSerialized] private Action _reloadCallback;
    [SerializeField] private bool inverse = false;
    private float _accumulator, _duration;

    private void Start() => gameObject.SetActive(false);

    private void Update()
    {
        if (!IsReloading) return;
        _accumulator += Time.deltaTime * Game.PowerUpFactor(PowerUpModel.PowerUp.Reload);
        var value = _accumulator / _duration;
        if (value is < 1 and > 0)
            slider.value = inverse ? 1 - value : value;
        else
        {
            _reloadCallback.Invoke();
            IsReloading = false;
            gameObject.SetActive(false);
        }
    }

    public void Reload(float duration, Action reloadCallback, float startAtPercentage = 0)
    {
        if (IsReloading) return;
        gameObject.SetActive(true);
        _reloadCallback = reloadCallback;
        _duration = duration;
        _accumulator = startAtPercentage * duration;
        IsReloading = true;
    }
}