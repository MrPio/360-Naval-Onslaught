using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionsFunctions;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Plane : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;

    [SerializeField] private GameObject dropBomb;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 5;
    [SerializeField] private bool isAlly;
    [SerializeField] private int dropAmount = 7;
    [SerializeField] private float dropWait = 0.2f;
    [SerializeField] private AudioClip planeClip;
    [SerializeField] private float dispersionHorizontal = 0.25f;
    private bool _dropping = false;

    private void Start()
    {
        var pos = MainCamera.MainCam.RandomBoundaryPoint() * 1.15f;
        transform.SetPositionAndRotation(pos, (-pos).ToQuaternion());
        rb.velocity = -(pos - Vector3.up * 1.25f) * speed * Random.Range(0.85f, 1.25f);
        MainCamera.AudioSource.PlayOneShot(planeClip);
    }

    private void OnBecameInvisible() => Destroy(gameObject);

    private IEnumerator DropBombs()
    {
        _dropping = true;
        for (var i = 0; i < dropAmount * (Game.IsSpecialWave ? 2 : 1); i++)
        {
            Vector2 dropPos = transform.position + Vector3.down * 0.15f;
            Instantiate(
                original: dropBomb,
                position: dropPos + new Vector2(dropPos.y, -dropPos.x) *
                Random.Range(-dispersionHorizontal, dispersionHorizontal),
                rotation: Quaternion.identity
            );
            yield return new WaitForSeconds(dropWait / (Game.IsSpecialWave ? 2 : 1));
        }

        _dropping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_dropping) return;
        if (collision.gameObject.CompareTag(isAlly ? "ship" : "base"))
            StartCoroutine(DropBombs());
    }
}