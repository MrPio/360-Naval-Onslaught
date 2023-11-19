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
        var isPc = !InputManager.Instance.HasJoystick() && !InputManager.IsMobile;
        keyboardKeys.ForEach(it => it.SetActive(isPc));
        gamepadKeys.ForEach(it => it.SetActive(!isPc));
    }
}