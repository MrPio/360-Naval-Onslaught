using System.Collections.Generic;
using ExtensionsFunctions;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private List<Sprite> spritesLight;
    [SerializeField] private List<Sprite> spritesDark;
    [Range(0f, 1f)] [SerializeField] private float darkProbability = 0.66f;
    [SerializeField] private Vector2 speedRange;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite =
            Random.Range(0, 1f) < darkProbability ? spritesDark.RandomItem() : spritesLight.RandomItem();
        GetComponent<Rigidbody2D>().velocity = Vector2.right * Random.Range(speedRange.x, speedRange.y);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}