using System.Collections.Generic;
using Managers;
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
                powerUpImage.sprite = powerUpSprites[(int)(Bubble.PowerUp)Game.PowerUp];
            else
                gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Game.PowerUpProgress > 1)
                gameObject.SetActive(false);
            else if (waveSpawner.isPaused)
                Game.PowerUpStart += Time.deltaTime;
            else
                powerUpSlider.value = 1-Game.PowerUpProgress;
        }
    }
