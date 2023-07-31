using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class HowToPlayMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> keyboardKeys;
    [SerializeField] private List<GameObject> gamepadKeys;

    private void OnEnable()
    {
        keyboardKeys.ForEach(it => it.SetActive(!InputManager.Instance.HasJoystick()));
        gamepadKeys.ForEach(it => it.SetActive(InputManager.Instance.HasJoystick()));
    }
}