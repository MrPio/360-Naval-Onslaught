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
    [SerializeField] private AudioSource audioSource, specialAudioSource;

    [SerializeField] private AudioClip winClip, mainMenuClip;
    [SerializeField] private List<AudioClip> levelsClip, specialLevelsClip;

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
        winMenu,
        mainMenuContinueGame;

    [NonSerialized] public bool isPaused;
    [SerializeField] private bool isDebug;
    [SerializeField] private List<Transform> specialPaths;
    [NonSerialized] private List<Transform> currentSpecialPoints;
    [NonSerialized] private int specialSpawned = 0;
    [SerializeField] private GameObject WarningPanel;
    [SerializeField] private Animator CameraAnimator, GlobalVolumeAnimator;


    public GameObject howToPlayMenu, overlay, mainMenu, gameOver, specialsMenu, shopMenu, bonusMenu;

    public void RestartGame()
    {
        GameStarted = Time.realtimeSinceStartup;
        InputManager.Reset();
        ShipPath.SpawnIndex = 0;
        Ship.Collisions.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        if (!isDebug)
        {
            baseMain.SetActive(false);
            baseHealthSlider.SetActive(false);
            newWave.SetActive(true);
            mainMenu.SetActive(true);
            gameOver.SetActive(false);
            specialsMenu.SetActive(false);
            GameObject.FindWithTag("pad_container").SetActive(InputManager.IsMobile);
            GameObject.Find("wave_container").GetComponent<RectTransform>().anchoredPosition =
                Vector2.right * (InputManager.IsMobile ? 150 : 0);
        }

        GameObject.FindWithTag("ocean").GetComponent<Renderer>().material =
            Resources.Load<Material>(Game.QualityOceanMaterials[Game.Quality]);

        audioSource.Stop();
        audioSource.volume = 1;
        audioSource.clip = mainMenuClip;
        audioSource.Play();

        if (isDebug)
            BeginWave();
        mainMenuContinueGame.SetActive(GameManager.HasSave());
    }

    private void FixedUpdate()
    {
        if (_model != null)
        {
            if (!isPaused)
                _accumulator += Time.fixedDeltaTime;

            if (!Game.IsSpecialWave)
            {
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
            else
            {
                // Spawn ship in special wave
                var baseDelay = 0.475;
                if (Game.SpecialWave == 2)
                    baseDelay = 0.35;
                if (_accumulator >= baseDelay - 0.25f * ((float)specialSpawned / currentSpecialPoints.Count) &&
                    specialSpawned < currentSpecialPoints.Count)
                {
                    SpawnSpecial();
                    _accumulator = 0;
                }
                // End special wave
                else if (_accumulator >= 7f && specialSpawned >= currentSpecialPoints.Count)
                {
                    _accumulator = -4f;
                    Game.SpecialWave = -1;
                    specialSpawned = 0;
                    specialAudioSource.Stop();
                    audioSource.volume = 0.5f;
                    audioSource.Play();
                    WarningPanel.SetActive(false);
                    Game.Ammo = Game.CurrentTurretModel.Ammo;
                    ammoContainer.GetComponentsInChildren<AmmoCounter>()[0].UpdateUI();
                    CameraAnimator.SetTrigger(Animator.StringToHash("stop_shake"));
                    MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(Vector2.zero, MainCamera.BaseOrthoSize);
                }
            }
        }
    }

    public void BeginSpecialWave()
    {
        if (Game.IsSpecialWave)
            return;

        var specialWave = Random.Range(0, specialPaths.Count);
        Game.SpecialWave = specialWave;
        Game.SpecialOccurInWave[Game.Wave] = true;
        var path = specialPaths[specialWave];
        currentSpecialPoints = path.GetComponentsInChildren<Transform>()
            .Where(tr => tr != path)
            .ToList();

        if (specialWave == 1)
            MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(Vector2.up * 3.5f, 8.5f);
        else if (specialWave == 2)
            MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(Vector2.right * 10f, 9.5f);
        else
            MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(Vector2.zero, 8.5f);
        DestroyAllShips();
        CameraAnimator.SetTrigger(Animator.StringToHash("shake"));

        _accumulator = -3f;
        Game.Ammo = Game.CurrentTurretModel.Ammo;
        ammoContainer.GetComponentsInChildren<AmmoCounter>()[0].UpdateUI();
        WarningPanel.SetActive(true);

        audioSource.Pause();
        specialAudioSource.clip = specialLevelsClip.RandomItem();
        specialAudioSource.Play();
    }

    private void DestroyAllShips()
    {
        // Destroy current ships
        foreach (var ship in GameObject.FindGameObjectsWithTag("ship").Select(it => it.GetComponent<Ship>()))
            if (ship.Invincible)
                Destroy(ship.gameObject);
            else
                ship.TakeDamage(int.MaxValue);
    }

    private void SpawnSpecial()
    {
        var pos = currentSpecialPoints[specialSpawned].position;
        var newShip = Instantiate(ship).GetComponent<Ship>();
        newShip.boxCollider.size = newShip.spriteRenderer.bounds.size;
        newShip.transform.SetPositionAndRotation(pos, pos.ToQuaternion());
        ++specialSpawned;
    }

    private void SpawnShip()
    {
        var pos = MainCamera.MainCam.RandomBoundaryPoint() * 1.3f;

        var newShip = Instantiate(ship).GetComponent<Ship>();
        newShip.boxCollider.size = newShip.spriteRenderer.bounds.size;
        newShip.transform.SetPositionAndRotation(pos, pos.ToQuaternion());

        var immediateSpawnChance = 0.1 + 0.185 * Game.WaveFactor;

        _nextSpawn = Random.Range(0f, 1f) < immediateSpawnChance
            ? 0f
            : (2.25f + Random.Range(0, 6f * (1 - 0.35f * Game.WaveFactor))) / _model.SpawnSpeedMultiply;
    }

    public void LoadGame()
    {
        GameManager.Load();
        DataManager.Load();
        overlay.GetComponent<Overlay>().OnEnd = () => EndWave(true);
        overlay.SetActive(true);
    }

    public void NewGame()
    {
        GameManager.Reset();
        DataManager.Reset();
        overlay.GetComponent<Overlay>().OnEnd = BeginWave;
        overlay.SetActive(true);
    }

    private void EndWave(bool isLoading = false)
    {
        if (!isLoading)
        {
            _model = null;
            Game.Score += 1000;
            ++Game.Wave;
            DestroyAllShips();
            foreach (var bullet in GameObject.FindGameObjectsWithTag("bullet"))
                Destroy(bullet);
            foreach (var laser in GameObject.FindGameObjectsWithTag("laser"))
                Destroy(laser);
            Game.Save();
            Data.Save();
        }

        baseMain.SetActive(false);
        ammoContainer.SetActive(false);
        moneyContainer.SetActive(false);
        specialsContainer.SetActive(false);
        scoreContainer.SetActive(false);
        waveContainer.SetActive(false);
        shopMenu.SetActive(false);
        bonusMenu.SetActive(false);
        winMenu.SetActive(false);
        GlobalVolumeAnimator.SetTrigger(Animator.StringToHash("fade"));

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
        audioSource.volume = 1f;
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
        if (isDebug)
            baseMain.SetActive(true);
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
        GlobalVolumeAnimator.SetTrigger(Animator.StringToHash("unfade"));

        // New Wave Sign
        if (!isDebug)
        {
            newWave.SetActive(true);
            newWave.transform.Find("new_wave_text").GetComponent<TextMeshProUGUI>().text = $"Wave {Game.Wave + 1}";
            newWave.GetComponent<Animator>().SetTrigger(Start1);
        }

        audioSource.Stop();
        audioSource.volume = 0.5f;
        audioSource.clip = levelsClip.RandomItem();
        audioSource.Play();
    }
}