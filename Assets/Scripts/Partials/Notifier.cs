using System;
using UnityEngine;

public class Notifier : MonoBehaviour
{
    public event Action Disabled, Enabled;

    private void OnEnable() => Enabled?.Invoke();
    private void OnDisable() => Disabled?.Invoke();
}