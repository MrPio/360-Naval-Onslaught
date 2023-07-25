using Managers;
using Model;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ship;
    private float _waveStart, _accumulator, _nextSpawn = 1f * 0f;
    private WaveModel _model;

    void Start()
    {
        BeginWave();
    }

    void FixedUpdate()
    {
        if (_model != null)
        {
            _accumulator += Time.fixedDeltaTime;
            if (!GameManager.Instance.CurrentWave.HasMore())
                EndWave();
            else if (_accumulator >= _nextSpawn)
                SpawnShip();
        }
    }

    private void SpawnShip()
    {
        Instantiate(ship);
        _nextSpawn = Random.Range(0, 2) switch
        {
            0 => 0f,
            _ => 0f+Random.Range(0, 4f * Mathf.Max(0, 1 - GameManager.Instance.Wave / DataManager.Instance.Waves.Length)),
        };
        _accumulator = 0;
    }

    private void EndWave()
    {
        _model = null;
        ++GameManager.Instance.Wave;
    }

    private void BeginWave()
    {
        _model = GameManager.Instance.CurrentWave;
        _waveStart = Time.time;
        _accumulator = 0;
    }
}