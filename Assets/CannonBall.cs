using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private const float Duration = 1;
    [SerializeField] private AudioClip cannonMiss, cannonHit;
    private float _accumulator;
    public Vector2 destination;

    private void Update()
    {
        _accumulator += Time.deltaTime;
        transform.position = destination * (_accumulator / Duration);
        if (_accumulator >= Duration)
        {
            Destroy(gameObject);
            
            //Spawn explosion HERE
        }
    }
}