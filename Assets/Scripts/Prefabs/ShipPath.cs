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
    [NonSerialized] public static int SpawnIndex;
    [NonSerialized] public ShipModel Model;
    [NonSerialized] public bool IsDead, Wait, IsFreezed;
    [NonSerialized] public float SpeedMultiplier = 1;
    [NonSerialized] public bool IsPathEnded = false;

    private int _pointIndex;
    private List<Vector2> _points = new();


    private void Start()
    {
        if (Model.HasPath)
        {
            var waveSpawner = GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>();
            var flip = new Vector2(Random.Range(0, 2) * 2 - 1, Random.Range(0, 2) * 2 - 1);
            var _path = paths[waveSpawner.PathsOrder[SpawnIndex % paths.Count]];
            SpawnIndex++;
            _points = _path.GetComponentsInChildren<Transform>()
                .Where(tr => tr != _path)
                .Select(tr => tr.position * flip * new Vector2(MainCamera.Width / 8.9f, 1))
                .ToList();
            transform.SetPositionAndRotation(_points[0], (_points[0] - _points[1]).ToQuaternion());
        }
    }

    private void FixedUpdate()
    {
        if (Model is { } && _pointIndex < _points.Count && !IsDead && !Wait && !IsFreezed)
        {
            var currentPos = (Vector2)transform.position;
            var newPos = Vector2.MoveTowards(
                current: currentPos,
                target: _points[_pointIndex],
                maxDistanceDelta: Model.Speed * SpeedMultiplier / 100f * Time.deltaTime
            );
            var newRotation = (currentPos - _points[_pointIndex]).ToQuaternion();
            transform.SetPositionAndRotation(
                position: newPos,
                rotation: Quaternion.Lerp(newRotation, transform.rotation, friction)
            );

            if (currentPos == _points[_pointIndex])
                ++_pointIndex;
            if (_pointIndex == _points.Count)
            {
                IsPathEnded = true;
                Model.EndPathCallback?.Invoke(gameObject);
            }
        }
    }

    public void AddPath(List<Vector2> points)
    {
        _pointIndex = 0;
        _points = points;
    }
}