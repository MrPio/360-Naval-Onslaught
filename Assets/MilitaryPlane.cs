using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionsFunctions;
using UnityEngine;

public class MilitaryPlane : MonoBehaviour
{
    [SerializeField] private GameObject dropBomb;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 5;

    private void Start()
    {
        var pos = MainCamera.MainCam.RandomBoundaryPoint() * 1.15f;
        transform.SetPositionAndRotation(pos, (-pos).ToQuaternion());
        rb.velocity = -(pos-Vector3.up*1.25f) * speed;
    }

    private void OnBecameInvisible() => Destroy(gameObject);

    private IEnumerator DropBombs()
    {
        for (var i = 0; i < 7; i++)
        {
            Instantiate(
                original: dropBomb,
                position: transform.position + Vector3.down * 0.15f,
                rotation: Quaternion.identity
            );
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("collision");
        if (collision.gameObject.CompareTag("base"))
            StartCoroutine(DropBombs());
    }
}