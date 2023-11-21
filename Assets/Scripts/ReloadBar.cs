using System;
using UnityEngine;
using UnityEngine.UI;

public class ReloadBar : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float offset;
    [SerializeField] private Slider slider;
    [NonSerialized] public bool IsReloading;
    [NonSerialized] public Action ReloadCallback;
    private float _accumulator, _duration;

    void Start()
    {
        transform.position = target.position + (offset > 0 ? Vector3.right : Vector3.up) * offset;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (IsReloading)
        {
            _accumulator += Time.deltaTime;
            var value = _accumulator / _duration;
            if (value < 1)
            {
                slider.value = value;
            }
            else
            {
                ReloadCallback.Invoke();
                IsReloading = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void Reload(float duration, Action reloadCallback)
    {
        if (!IsReloading)
        {
            gameObject.SetActive(true);
            ReloadCallback = reloadCallback;
            _duration = duration;
            _accumulator = 0;
            IsReloading = true;
        }
    }
}