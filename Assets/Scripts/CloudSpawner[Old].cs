using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Range(0.5f, 12f)] [SerializeField] private float frequency = 4;
    [SerializeField] private GameObject cloud;
    private float _accumulator, _nextSpawn = 1f;


    void Update()
    {
        _accumulator += Time.deltaTime;
        if (_accumulator >= _nextSpawn)
        {
            _accumulator = 0;
            _nextSpawn = Random.Range(0f, 10f) / frequency;
            Instantiate(
                original: cloud,
                position: new Vector2(
                    x: -MainCamera.Width - 4f,
                    y: Random.Range(-MainCamera.Height-1f, MainCamera.Height+1f)
                ),
                rotation: Quaternion.identity
            );
        }
    }
}