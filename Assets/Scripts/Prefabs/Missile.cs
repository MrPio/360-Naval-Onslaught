using System;
using System.Linq;
using Interfaces;
using Managers;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float Duration = 1;
    [NonSerialized] public int Damage = 0;
    [SerializeField] private AudioClip cannonMiss, cannonHit;
    [SerializeField] private GameObject explosion, splash;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isEnemy = true, destroyWithParent, freezeX, freezeY;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float hitRange = 0.35f;
    private float _accumulator;
    [NonSerialized] public Vector2 Destination, StartPosition;

    private void Update()
    {
        _accumulator += Time.deltaTime;
        var currentPos = Vector2.Lerp(StartPosition, Destination, animationCurve.Evaluate(_accumulator / Duration));
        currentPos = new Vector2(
            freezeX ? transform.localPosition.x : currentPos.x,
            freezeY ? transform.localPosition.y : currentPos.y
        );
        transform.localPosition = currentPos;
        if (_accumulator >= Duration)
        {
            // Explosion + Splash
            Instantiate(splash).transform.position = transform.position;
            var hit = false;

            // Check collisions
            foreach (var go in Physics2D
                         .OverlapCircleAll((Vector2)transform.position + Vector2.down * 0.25f, hitRange)
                         .Where(col => (isEnemy ? new[] { "base" } : new[] { "ship", "power_up" }).Any(col.CompareTag))
                    )
            {
                hit = true;
                go.GetComponent<IDamageable>().TakeDamage(Damage);
                Instantiate(explosion).transform.position = transform.position;
            }

            MainCamera.AudioSource.PlayOneShot(hit ? cannonHit : cannonMiss);

            Destroy(destroyWithParent ? transform.parent.gameObject : gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.down * 0.25f, hitRange);
    }

    public void SetMissile(Sprite missileSprite)
    {
        spriteRenderer.sprite = missileSprite;
    }
}