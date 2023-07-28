using System;
using TMPro;
using UnityEngine;

public class EmpText : MonoBehaviour
{
    private const int EmpTime = 10;
    [SerializeField] private TextMeshProUGUI empText;
    [SerializeField] private ShipPath shipPath;
    private float _accumulator;
    private float _duration = EmpTime;
    private int _lastSecond;
    
    private void OnEnable()
    {
        _accumulator = 0;
        _duration = 0;
        _lastSecond = 0;
        shipPath.Wait = true;
        shipPath.gameObject.GetComponent<Renderer>().material.color = new Color(0.5f, 0.55f, 0.9f, 0.65f);
        EMP();
    }

    public void EMP()
    {
        _duration += EmpTime;
    }

    private void Update()
    {
        _accumulator += Time.deltaTime;

        transform.rotation = MainCamera.MainCam.transform.rotation;

        if ((int)_accumulator != _lastSecond)
        {
            _lastSecond = (int)_accumulator;
            empText.text = (_duration - _lastSecond).ToString();
        }

        if (_accumulator >= _duration)
        {
            shipPath.Wait = false;
            shipPath.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1, 1f);
            shipPath.gameObject.GetComponent<Ship>().IsFreezed = false;
            gameObject.SetActive(false);
        }
    }
}