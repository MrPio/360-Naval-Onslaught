using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private const float Duration = 1;
    [SerializeField] private AudioClip cannonMiss, cannonHit;
    private float _accumulator;
    [NonSerialized] public Vector2 Destination;

    private void Update()
    {
        _accumulator += Time.deltaTime;
        transform.position = Destination * (_accumulator / Duration);
        if (_accumulator >= Duration)
        {
            Destroy(gameObject);
            
            //Spawn explosion HERE
        }
    }
}