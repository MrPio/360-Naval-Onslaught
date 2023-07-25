using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class ReloadBar : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float offset;
    [SerializeField] private Slider slider;
    [SerializeField] private AmmoCounter ammoCounter;
    [SerializeField] private AudioClip reloadFinish;
    [NonSerialized] public bool IsReloading;
    private float _accumulator, _duration;

    void Start()
    {
        transform.position = target.position + Vector3.right * offset;
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
                GameManager.Instance.Ammo = GameManager.Instance.CurrentTurretModel.Ammo;
                ammoCounter.UpdateUI();
                IsReloading = false;
                gameObject.SetActive(false);
                MainCamera.AudioSource.PlayOneShot(reloadFinish);
            }
        }
    }

    public void Reload()
    {
        if (!IsReloading)
        {
            gameObject.SetActive(true);
            _duration = 100f / GameManager.Instance.CurrentTurretModel.Reload;
            _accumulator = 0;
            IsReloading = true;
        }
    }
}