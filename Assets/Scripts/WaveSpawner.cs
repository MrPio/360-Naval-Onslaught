using Managers;
using Model;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ship;
    private float _waveStart, _accumulator, _nextSpawn = 1f;
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
            if (!PlayerManager.Instance.CurrentWave.HasMore())
                EndWave();
            else if (_accumulator >= _nextSpawn)
                SpawnShip();
        }
    }

    private void SpawnShip()
    {
        Instantiate(ship);
        _nextSpawn = Random.Range(0f, 2f + 4f * Mathf.Max(0, 1 - PlayerManager.Instance.Wave / 20f));
        _accumulator = 0;
    }

    private void EndWave()
    {
        _model = null;
        ++PlayerManager.Instance.Wave;
    }

    private void BeginWave()
    {
        _model = PlayerManager.Instance.CurrentWave;
        _waveStart = Time.time;
        _accumulator = 0;
    }
}