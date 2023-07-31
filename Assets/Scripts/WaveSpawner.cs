using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Managers;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class WaveSpawner : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    private static DataManager Data => DataManager.Instance;
    public static float GameStarted;

    [SerializeField] private GameObject ship;
    private float _waveStart, _accumulator, _nextSpawn;
    private WaveModel _model;
    [SerializeField] private int pathsSize = 1;
    [NonSerialized] public List<int> PathsOrder;
    private static readonly int Start1 = Animator.StringToHash("start");
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip winClip, mainMenuClip;
    [SerializeField] private List<AudioClip> levelsClip;

    [SerializeField] private GameObject waveCounter,
        baseMain,
        ammoContainer,
        moneyContainer,
        specialsContainer,
        scoreContainer,
        waveContainer,
        newWave,
        baseHealthSlider,
        accuracyMenu,
        winMenu;

    [NonSerialized] public bool isPaused;

    public GameObject howToPlayMenu, overlay, mainMenu, gameOver, specialsMenu, shopMenu, bonusMenu;

    public void RestartGame()
    {
        GameStarted = Time.realtimeSinceStartup;
        GameManager.Reset();
        DataManager.Reset();
        InputManager.Reset();
        ShipPath.SpawnIndex = 0;
        Ship.Collisions.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        // TODO UNCOMMENT
        baseMain.SetActive(false);
        baseHealthSlider.SetActive(false);
        newWave.SetActive(true);
        mainMenu.SetActive(true);
        gameOver.SetActive(false);
        specialsMenu.SetActive(false);

        audioSource.Stop();
        audioSource.clip = mainMenuClip;
        audioSource.Play();


        // TODO COMMENT
        // BeginWave();
    }

    private void FixedUpdate()
    {
        if (_model != null)
        {
            if (!isPaused)
                _accumulator += Time.fixedDeltaTime;

            if (!Game.CurrentWave.HasMore() && _accumulator > 4f &&
                Game.CurrentWave.Destroyed >= Game.CurrentWave.ShipsCount)
                EndWave();
            else if (!Game.CurrentWave.HasMore() && Game.CurrentWave.Destroyed < Game.CurrentWave.ShipsCount)
                _accumulator = 0;
            else if (Game.CurrentWave.HasMore() && _accumulator >= _nextSpawn)
            {
                _accumulator = 0;
                SpawnShip();
            }
        }
    }

    private void SpawnShip()
    {
        var pos = MainCamera.MainCam.RandomBoundaryPoint() * 1.3f;

        var newShip = Instantiate(ship).GetComponent<Ship>();
        newShip.boxCollider.size = newShip.spriteRenderer.bounds.size;
        newShip.transform.SetPositionAndRotation(pos, pos.ToQuaternion());
        newShip.CurrentIndex = Game.CurrentWave.Spawned;

        ++Game.CurrentWave.Spawned;

        var immediateSpawnChance = 0.1 + 0.25 * Game.WaveFactor;

        _nextSpawn = Random.Range(0f, 1f) < immediateSpawnChance
            ? 0f
            : (2f + Random.Range(0, 6f * (1 - 0.45f*Game.WaveFactor))) / _model.SpawnSpeedMultiply;
    }

    private void EndWave()  
    {
        _model = null;
        Game.Score += 1000;
        ++Game.Wave;
        baseMain.SetActive(false);
        ammoContainer.SetActive(false);
        moneyContainer.SetActive(false);
        specialsContainer.SetActive(false);
        scoreContainer.SetActive(false);
        waveContainer.SetActive(false);
        shopMenu.SetActive(false);
        bonusMenu.SetActive(false);
        winMenu.SetActive(false);

        if (Game.Wave >= Data.Waves.Length)
        {
            // Show Win Menu
            winMenu.SetActive(true);
            winMenu.transform.Find("score_text").GetComponent<TextMeshProUGUI>().text =
                Game.Score.ToString("N0") + " pts";
        }
        else
            accuracyMenu.SetActive(true);

        audioSource.Stop();
        audioSource.clip = winClip;
        audioSource.Play();
    }

    public void BeginWave()
    {
        Game.Ammo = Game.CurrentTurretModel.Ammo;
        waveCounter.GetComponent<TextMeshProUGUI>().text = (Game.Wave + 1).ToString();
        ammoContainer.SetActive(true);
        moneyContainer.SetActive(true);
        specialsContainer.SetActive(true);
        scoreContainer.SetActive(true);
        waveContainer.SetActive(true);
        shopMenu.SetActive(false);
        accuracyMenu.SetActive(false);
        bonusMenu.SetActive(false);
        winMenu.SetActive(false);
        baseHealthSlider.SetActive(false);
        // baseMain.SetActive(true);//TODO comment
        _model = Game.CurrentWave;
        _waveStart = Time.time;
        _accumulator = 0;
        _nextSpawn = 2f;
        PathsOrder = Enumerable.Range(0, pathsSize).ToList();
        PathsOrder.Shuffle();
        Game.CurrentWaveTurretFired = 0;
        Game.CurrentWaveTurretHit = 0;
        Game.CurrentWaveCannonFired = 0;
        Game.CurrentWaveCannonHit = 0;

        // TODO UNCOMMENT
        // New Wave Sign
        newWave.SetActive(true);
        newWave.transform.Find("new_wave_text").GetComponent<TextMeshProUGUI>().text = $"Wave {Game.Wave + 1}";
        newWave.GetComponent<Animator>().SetTrigger(Start1);

        audioSource.Stop();
        audioSource.clip = levelsClip.RandomItem();
        audioSource.Play();
    }
}