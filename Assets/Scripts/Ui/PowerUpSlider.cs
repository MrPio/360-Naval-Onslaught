using System.Collections.Generic;
using Managers;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;


public class PowerUpSlider : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    [SerializeField] private Slider powerUpSlider;
    [SerializeField] private Image powerUpImage;
    [SerializeField] private List<Sprite> powerUpSprites;
    [SerializeField] private WaveSpawner waveSpawner;

    private void OnEnable()
    {
        if (Game.PowerUp is { })
            powerUpImage.sprite = Resources.Load<Sprite>(Game.PowerUp.Sprite);
        else
            gameObject.SetActive(false);
    }

    private void Update()
    {
        var progress = Game.PowerUpProgress;
        if (progress > 1)
            gameObject.SetActive(false);
        else if (waveSpawner.isPaused)
            Game.PowerUpStart += Time.deltaTime;
        else
            powerUpSlider.value = 1 - progress;
    }
}