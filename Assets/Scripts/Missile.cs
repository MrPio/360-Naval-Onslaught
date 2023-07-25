using System;
using System.Linq;
using Managers;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float Duration = 1;
    [NonSerialized] public int Damage = 0;
    [SerializeField] private AudioClip cannonMiss, cannonHit;
    [SerializeField] private GameObject explosion, splash;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float _accumulator;
    [NonSerialized] public Vector2 Destination, StartPosition;

    private void Update()
    {
        _accumulator += Time.deltaTime;
        var currentPos = Vector2.Lerp(StartPosition, Destination, _accumulator / Duration);
        transform.position = currentPos;
        if (_accumulator >= Duration)
        {
            // Explosion + Splash
            Instantiate(
                original: splash,
                position: currentPos,
                rotation: Quaternion.identity
            );
            var hit = false;

            // Check collisions
            foreach (var player in Physics2D
                         .OverlapCircleAll(currentPos, 0.35f)
                         .Where(col => col.CompareTag("base"))
                    )
            {
                hit = true;
                player.GetComponent<Base>().TakeDamage(Damage);
                Instantiate(
                    original: explosion,
                    position: currentPos,
                    rotation: Quaternion.identity
                );
            }

            MainCamera.AudioSource.PlayOneShot(hit ? cannonHit : cannonMiss);

            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GameManager.Instance.CurrentCannonModel.Radius / 100f);
    }

    public void SetMissile(Sprite missileSprite)
    {
        spriteRenderer.sprite = missileSprite;
    }
}