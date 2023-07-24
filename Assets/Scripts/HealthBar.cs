using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float offset;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject fillArea;

    private void Update()
    {
        transform.SetPositionAndRotation(
            position: target.position + Vector3.up * offset,
            rotation: MainCamera.mainCam.transform.rotation
        );
    }

    public void setValue(float value)
    {
        slider.value = value;
        if (value < 0.02) fillArea.SetActive(false);
    }
}