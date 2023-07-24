using System.Collections.Generic;
using ExtensionsFunctions;
using UnityEngine;

public class Speedboat : MonoBehaviour
{
    void Start()
    {
        var pos = MainCamera.mainCam.RandomBoundaryPoint()*1.1f;
        transform.SetPositionAndRotation(pos,pos.toQuaternion());
        
        GetComponent<ShipPath>().AddPath(
            Random.Range(0, 2) == 0
                ? new List<Vector3> { Vector3.zero }
                : new List<Vector3>
                {
                    Quaternion.AngleAxis(Random.Range(-30f, 30f), Vector3.forward) * transform.position,
                    Vector3.zero
                }
        );
    }

    // Update is called once per frame
    void Update()
    {
    }
}