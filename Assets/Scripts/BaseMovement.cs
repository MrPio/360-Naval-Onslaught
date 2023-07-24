using System.Collections;
using System.Collections.Generic;
using ExtensionsFunctions;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    [SerializeField] private GameObject cannon;
    [SerializeField] private GameObject turret;

    void Update()
    {
        transform.rotation = MainCamera.mainCam.AngleToMouse(transform.position);
    }
}