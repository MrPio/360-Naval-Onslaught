using System;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class GlitchController : MonoBehaviour
{
    [SerializeField] private Material material;
    [FormerlySerializedAs("strenght")] [SerializeField] private float strength;
    private static readonly int Strength = Shader.PropertyToID("_Strenght");
    private float _oldStrength;

    private void Update()
    {
        if (Math.Abs(_oldStrength - strength) > 0.0001)
        {
            material.SetFloat(Strength, strength);
            _oldStrength = strength;
        }
    }
}