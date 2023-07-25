using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionsFunctions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private Vector2 speedRange;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites.RandomItem();
        GetComponent<Rigidbody2D>().velocity = Vector2.right * Random.Range(speedRange.x, speedRange.y);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}