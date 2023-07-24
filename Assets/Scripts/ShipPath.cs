using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Model;
using UnityEngine;

public class ShipPath : MonoBehaviour
{
    [SerializeField] private Transform path;
    [SerializeField] [Range(0.9f, 1f)] private float friction = 0.99f;
    public ShipModel Model;
    public bool dead;

    private int _index;
    private List<Vector3> _points = new();

    private void Start()
    {
        if (path != null)
            _points = path.GetComponentsInChildren<Transform>()
                .Where(tr => tr != path)
                .Select(tr => tr.position)
                .ToList();
    }

    private void FixedUpdate()
    {
        if (path is { } && Model is { } && !dead && _index <= _points.Count - 1)
        {
            var currentPos = transform.position;
            var newPos = Vector2.MoveTowards(
                current: currentPos,
                target: _points[_index],
                maxDistanceDelta: Model.Speed / 100f * Time.deltaTime
            );
            var newRotation = (currentPos - _points[_index]).toQuaternion();
            transform.SetPositionAndRotation(
                position: newPos,
                rotation: Quaternion.Lerp(newRotation, transform.rotation, friction)
            );

            if (currentPos == _points[_index])
                ++_index;
        }
    }

    public void AddPath(List<Vector3> points)
    {
        _index = 0;
        this._points = points;
    }
}