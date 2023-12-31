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

    [SerializeField] private AudioClip winClip, mainMenuClip, lastLevelClip;
    [SerializeField] private List<AudioClip> levelsClip, specialLevelsClip;

    [SerializeField] private GameObject
        waveCounter,
        ammoContainer,
        moneyContainer,
        specialsContainer,
        scoreContainer,
        waveContainer,
        newWave,
        baseHealthSlider,
        accuracyMenu,
        winMenu,
        mainMenuContinueGame,
        mobileShopConfirm,
        difficultyMenu,
        wheel;

    public GameObject
        overlay,
        baseMain,
        mainMenu,
        gameOver,
        specialsMenu,
        shopMenu,
        bonusMenu,
        howToPlayMenu;

    [NonSerialized] public bool isPaused;
    [SerializeField] private bool isDebug;
    [SerializeField] private List<Transform> specialPaths;
    [NonSerialized] private List<Transform> currentSpecialPoints;
    [NonSerialized] private int specialSpawned = 0;
    [SerializeField] private GameObject WarningPanel;
    [SerializeField] private Animator CameraAnimator, GlobalVolumeAnimator;


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
            howToPlayMenu.SetActive(false);
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
                    EndSpecialWave();
            }
        }
    }

    public void SpecialShipSpawned()
    {
        _accumulator -= 3;
    }

    public void BeginSpecialWave()
    {
        if (Game.IsSpecialWave)
            return;

        var specialWave = Random.Range(0, specialPaths.Count);
        Game.SpecialWave = specialWave;
        Game.SpecialOccurInWave[Game.Wave] = true;

        // Retrieve the special spawn points list
        var path = specialPaths[specialWave];

        // Retrieve the spawn points transforms
        currentSpecialPoints = path.GetComponentsInChildren<Transform>()
            .Where(tr => tr != path)
            .ToList();

        // Move the Camera according to the special wave type (TODO move to SpecialWaveModel class)
        if (path.name is "n")
            MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(Vector2.up * 3.5f, 8.5f);
        if (path.name is "e")
            MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(Vector2.right * 10f, 8.5f);
        if (path.name is "s")
            MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(Vector2.down * 3.5f, 8.5f);
        if (path.name is "ne")
            MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(Vector2.one * 3.5f, 9f);
        if (path.name is "se")
            MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(new Vector2(1, -1) * 3.5f, 9f);
        if (path.name is "nesw")
            MainCamera.MainCam.GetComponent<MainCamera>().TransitionTo(Vector2.zero, 8.5f);
        DestroyAllShips();
        CameraAnimator.SetTrigger(Animator.StringToHash("shake"));

        _accumulator = -2f;
        Game.Ammo = Game.CurrentTurretModel.Ammo;
        ammoContainer.GetComponentsInChildren<AmmoCounter>()[0].UpdateUI();
        WarningPanel.SetActive(true);

        audioSource.Pause();
        specialAudioSource.clip = specialLevelsClip.RandomItem();
        specialAudioSource.Play();
    }


    private void EndSpecialWave()
    {
        isPaused = true;
        _accumulator = -3f;
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
        wheel.SetActive(true);
    }

    private void DestroyAllShips()
    {
        // Destroy current ships
        foreach (var ship in GameObject.FindGameObjectsWithTag("ship").Select(it => it.GetComponent<Ship>()))
            if (ship.Invincible)
            {
                Destroy(ship.gameObject);
                ++Game.CurrentWave.Destroyed;
            }
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
        var spawnPoint = MainCamera.MainCam.RandomBoundaryPoint() * 1.3f;

        var newShip = Instantiate(ship).GetComponent<Ship>();
        newShip.boxCollider.size = newShip.spriteRenderer.bounds.size;
        newShip.transform.SetPositionAndRotation(spawnPoint, spawnPoint.ToQuaternion());

        var immediateSpawnChance = 0.1 + 0.185 * Game.WaveFactor;

        _nextSpawn = Random.Range(0f, 1f) < immediateSpawnChance
            ? 0.45f
            : (2.45f + Random.Range(0, 6f * (1 - 0.35f * Game.WaveFactor))) / _model.SpawnSpeedMultiply;
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
        difficultyMenu.SetActive(true);
        difficultyMenu.GetComponent<DifficultyMenu>().OnClose = () =>
        {
            overlay.GetComponent<Overlay>().OnEnd = BeginWave;
            overlay.SetActive(true);
        };
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
        mobileShopConfirm.SetActive(false);
        accuracyMenu.SetActive(false);
        bonusMenu.SetActive(false);
        winMenu.SetActive(false);
        baseHealthSlider.SetActive(false);
        if (isDebug)
            baseMain.SetActive(true);
        _model = Game.CurrentWave;
        GameObject.FindWithTag("cloud_manager").GetComponent<CloudManager>().SetStrength();
        _waveStart = Time.time;
        _accumulator = -3f;
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
        audioSource.clip = Game.Wave == Data.Waves.Length - 1 ? lastLevelClip : levelsClip.RandomItem();
        audioSource.Play();
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
        mobileShopConfirm.SetActive(false);
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


}