using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Model;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ShipPath : MonoBehaviour
{
    [SerializeField] [Range(0.9f, 1f)] private float friction = 0.99f;
    [SerializeField] private List<Transform> paths;
    [NonSerialized] private static int _spawnIndex;
    [NonSerialized] public ShipModel Model;
    [NonSerialized] public bool Dead;
    [NonSerialized] public bool Wait;

    private int _pointIndex;
    private List<Vector2> _points = new();


    private void Start()
    {
        if (Model.HasPath)
        {
            var waveSpawner = GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>();
            var flip = new Vector2(Random.Range(0, 2) * 2 - 1, Random.Range(0, 2) * 2 - 1);
            var _path = paths[waveSpawner.PathsOrder[_spawnIndex % paths.Count]];
            _spawnIndex++;
            _points = _path.GetComponentsInChildren<Transform>()
                .Where(tr => tr != _path)
                .Select(tr => tr.position * flip * new Vector2(MainCamera.Width / 8.9f, 1))
                .ToList();
            transform.SetPositionAndRotation(_points[0], (_points[0] - _points[1]).ToQuaternion());
        }
    }

    private void FixedUpdate()
    {
        if (Model is { } && !Dead && _pointIndex < _points.Count && !Wait)
        {
            var currentPos = (Vector2)transform.position;
            var newPos = Vector2.MoveTowards(
                current: currentPos,
                target: _points[_pointIndex],
                maxDistanceDelta: Model.Speed / 100f * Time.deltaTime
            );
            var newRotation = (currentPos - _points[_pointIndex]).ToQuaternion();
            transform.SetPositionAndRotation(
                position: newPos,
                rotation: Quaternion.Lerp(newRotation, transform.rotation, friction)
            );

            if (currentPos == _points[_pointIndex])
                ++_pointIndex;
            if (_pointIndex == _points.Count)
                Model.EndPathCallback?.Invoke(gameObject);
        }
    }

    public void AddPath(List<Vector2> points)
    {
        _pointIndex = 0;
        _points = points;
    }
}