using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using UnityEngine;

public class ShipPath : MonoBehaviour
{
    [SerializeField] private Transform path;
    [SerializeField] private float speed = 1;
    [SerializeField] [Range(0.9f, 1f)] private float friction = 0.99f;

    private int index;
    private List<Vector3> points;

    private void Start()
    {
        if (path != null)
            points = path.GetComponentsInChildren<Transform>()
                .Where(tr => tr != path)
                .Select(tr => tr.position)
                .ToList();
    }

    private void FixedUpdate()
    {
        if (path is { } && index <= points.Count - 1)
        {
            var currentPos = transform.position;
            var newPos = Vector2.MoveTowards(
                current: currentPos,
                target: points[index],
                maxDistanceDelta: speed * Time.deltaTime
            );
            var newRotation = (currentPos - points[index]).toQuaternion();
            transform.SetPositionAndRotation(
                position: newPos,
                rotation: Quaternion.Lerp(newRotation, transform.rotation, friction)
            );

            if (currentPos == points[index])
                ++index;
        }
    }

    public void AddPath(List<Vector3> points)
    {
        index = 0;
        this.points = points;
    }
}