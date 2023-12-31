using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float offset;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject fillArea;
    [SerializeField] private bool adjustRotationAndPosition = true;

    private void Start()
    {
        if (Mathf.Abs(offset) > 0.001f)
            transform.position = target.position + Vector3.up * offset;
    }

    private void Update()
    {
        if (adjustRotationAndPosition)
        {
            transform.SetPositionAndRotation(
                position: target.position + Vector3.up * offset,
                rotation: MainCamera.MainCam.transform.rotation
            );
        }
    }

    public void SetValue(float value)
    {
        gameObject.SetActive(true);
        slider.value = value;
        if (value < 0.02) fillArea.SetActive(false);
    }
}