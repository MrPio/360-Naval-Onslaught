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

    [SerializeField] private GameObject ship;
    private float _waveStart, _accumulator, _nextSpawn;
    private WaveModel _model;
    [SerializeField] private int pathsSize = 1;
    [NonSerialized] public List<int> PathsOrder;
    private static readonly int Start1 = Animator.StringToHash("start");


    [SerializeField] private GameObject waveCounter,
        baseMain,
        ammoContainer,
        moneyContainer,
        waveContainer,
        shopMenu,
        newWave,
        baseHealthSlider;

    public GameObject howToPlayMenu,overlay,mainMenu,gameOver;

    public void RestartGame()
    {
        GameManager.Reset();
        DataManager.Reset();
        ShipPath.SpawnIndex = 0;
        Ship.Collisions.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void Start()
    {
        // TODO UNCOMMENT
        /*baseMain.SetActive(false);
        baseHealthSlider.SetActive(false);
        newWave.SetActive(true);
        mainMenu.SetActive(true);
        gameOver.SetActive(false);*/
        
        // TODO COMMENT
        BeginWave();
    }

    private void FixedUpdate()
    {
        if (_model != null)
        {
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
        Instantiate(ship).GetComponent<Ship>().CurrentIndex = Game.CurrentWave.Spawned;
        ++Game.CurrentWave.Spawned;

        var immediateSpawnChance = 0.1 + 0.3 * Game.WaveFactor;

        _nextSpawn = Random.Range(0f, 1f) < immediateSpawnChance
            ? 0f
            : 3f + Random.Range(0, 8f * (1 - Game.WaveFactor));
    }

    private void EndWave()
    {
        _model = null;
        Game.Score += 1000;
        ++Game.Wave;
        baseMain.SetActive(false);
        ammoContainer.SetActive(false);
        moneyContainer.SetActive(false);
        waveContainer.SetActive(false);
        shopMenu.SetActive(true);
    }

    public void BeginWave()
    {
        waveCounter.GetComponent<TextMeshProUGUI>().text = (Game.Wave+1).ToString();
        ammoContainer.SetActive(true);
        moneyContainer.SetActive(true);
        waveContainer.SetActive(true);
        shopMenu.SetActive(false);
        baseHealthSlider.SetActive(false);
        _model = Game.CurrentWave;
        _waveStart = Time.time;
        _accumulator = 0;
        _nextSpawn = 7f;
        PathsOrder = Enumerable.Range(0, pathsSize).ToList();
        PathsOrder.Shuffle();
        
        // TODO UNCOMMENT
        /*newWave.SetActive(true);
        newWave.transform.Find("new_wave_text").GetComponent<TextMeshProUGUI>().text = $"Wave {Game.Wave+1}";
        newWave.GetComponent<Animator>().SetTrigger(Start1);*/
    }
}