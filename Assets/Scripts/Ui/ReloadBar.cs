using System;
using JetBrains.Annotations;
using Managers;
using Model;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ReloadBar : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;

    [SerializeField] private Slider slider;
    [SerializeField] [CanBeNull] private Stack stackParent;
    [NonSerialized] public bool IsReloading;
    [NonSerialized] private Action _reloadCallback;
    private float _accumulator, _duration;

    private void Start() => gameObject.SetActive(false);

    private void Update()
    {
        if (!IsReloading) return;
        _accumulator += Time.deltaTime* Game.PowerUpFactor(PowerUpModel.PowerUp.Reload);
        var value = _accumulator / _duration;
        if (value < 1)
            slider.value = value;
        else
        {
            _reloadCallback.Invoke();
            IsReloading = false;
            gameObject.SetActive(false);
        }
    }

    public void Reload(float duration, Action reloadCallback)
    {
        if (IsReloading) return;
        gameObject.SetActive(true);
        _reloadCallback = reloadCallback;
        _duration = duration;
        _accumulator = 0;
        IsReloading = true;
    }

    private void OnEnable() => stackParent?.UpdateUI();
    private void OnDisable() => stackParent?.UpdateUI();
}