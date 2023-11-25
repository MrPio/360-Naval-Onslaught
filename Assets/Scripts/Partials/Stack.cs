using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class Stack : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private List<Transform> children;
    [SerializeField] private float gap = 1, offset;

    public void UpdateUI()
    {
        transform.position = target.position + Vector3.up * offset;

        var located = 0;
        foreach (var child in children.Where(child => child.gameObject.activeSelf))
            child.localPosition = Vector3.up * (gap * located++);
    }

    private void Start() => UpdateUI();
}