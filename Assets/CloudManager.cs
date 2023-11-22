using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CloudManager : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> particleSystems;
    private readonly List<float> _baseStrengths = new();

    private void Start()
    {
        _baseStrengths.AddRange(particleSystems.Select(it => it.emission.rateOverTimeMultiplier));
    }

    public void SetStrength(float strength = -1)
    {
        if (strength < 0)
            strength = Random.Range(0.75f, 1.5f);
        for (var i = 0; i < particleSystems.Count; i++)
        {
            var emission = particleSystems[i].emission;
            emission.rateOverTimeMultiplier = _baseStrengths[i] * strength;
        }

        ;
    }
}