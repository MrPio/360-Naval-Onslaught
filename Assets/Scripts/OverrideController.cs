using System;
using Managers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class OverrideController : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;

    [SerializeField] private GameObject overheatingWarning;
    [SerializeField] private Slider overheatingSlider;
    [SerializeField] private Turret turret;
    [SerializeField] private SpriteRenderer turretSpriteRenderer, cannonSpriteRenderer, baseSpriteRenderer;
    [SerializeField] private AudioClip overriding, overheatAlarm, overheating;
    [SerializeField] private Animator overrideAnimator, turretAnimator, cannonAnimator, baseAnimator;
    [SerializeField] private AudioSource bulletAudioSource;
    private float Percentage => Game.OverrideAmount / Game.OverrideDuration;

    private void Update()
    {
        if (Game.HasOverride)
        {
            Game.OverrideAmount += Time.deltaTime;
            overheatingSlider.value = Percentage;
            turretSpriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.Pow(Percentage, 2f));
            cannonSpriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.Pow(Percentage * 0.5f, 2f));
            baseSpriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.Pow(Percentage * 0.5f, 2f));


            // 80%, almost overheating
            if (Percentage > 0.66f && !overheatingWarning.activeSelf)
            {
                overheatingWarning.SetActive(true);
                MainCamera.AudioSource.PlayOneShot(overheatAlarm);
            }

            // >100% overheating
            if (Percentage >= 1f)
            {
                Game.IsOverheated = true;
                Override(false);
                turret.Reload();
                MainCamera.AudioSource.PlayOneShot(overheating);
            }
        }
        else if (Game.OverrideAmount > 0.001f)
        {
            Game.OverrideAmount -= Time.deltaTime / Game.OverheatingDurationFactor;
            if (Game.OverrideAmount < 0.001)
            {
                Game.IsOverheated = false;
                overheatingSlider.gameObject.SetActive(false);
                turretSpriteRenderer.color = Color.white;
                turretAnimator.enabled = true;
                cannonAnimator.enabled = true;
                baseAnimator.enabled = true;
                baseAnimator.Rebind();
                cannonAnimator.Rebind();
                turretAnimator.Rebind();
                return;
            }

            overheatingSlider.value = Percentage;
            turretSpriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.Pow(Percentage, 2f));
            cannonSpriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.Pow(Percentage * 0.5f, 2f));
            baseSpriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.Pow(Percentage * 0.5f, 2f));
        }
    }

    public void Override(bool value)
    {
        if (!value && Game.HasOverride)
        {
            overrideAnimator.SetTrigger(Animator.StringToHash("reverse"));
            Game.HasOverride = false;
            overheatingWarning.SetActive(false);
            bulletAudioSource.pitch = 1f;
        }
        else if (value && !Game.HasOverride && !Game.IsOverheated && !turret.reloadBar.IsReloading)
        {
            Game.HasOverride = true;
            turretAnimator.enabled = false;
            cannonAnimator.enabled = false;
            baseAnimator.enabled = false;
            overheatingSlider.gameObject.SetActive(true);
            overrideAnimator.SetTrigger(Animator.StringToHash("start"));
            MainCamera.AudioSource.PlayOneShot(overriding);
            bulletAudioSource.pitch = 1.4f;
        }
    }
}