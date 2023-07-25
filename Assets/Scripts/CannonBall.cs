using System;
using System.Linq;
using Managers;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float Duration = 1;
    [SerializeField] private AudioClip cannonMiss, cannonHit;
    [SerializeField] private GameObject explosion, splash;
    private float _accumulator;
    [NonSerialized] public Vector2 Destination;

    private void Update()
    {
        _accumulator += Time.deltaTime;
        var currentPos = Destination * (_accumulator / Duration);
        transform.position = currentPos;
        if (_accumulator >= Duration)
        {
            // Explosion + Splash
            Instantiate(
                original: splash,
                position: currentPos,
                rotation: Quaternion.identity
            );
            Instantiate(
                original: explosion,
                position: currentPos,
                rotation: Quaternion.identity
            );
            var hit = false;

            // Check collisions
            foreach (var ship in Physics2D
                         .OverlapCircleAll(currentPos, GameManager.Instance.CurrentCannonModel.Radius / 100f)
                         .Where(col => col.CompareTag("ship"))
                    )
            {
                hit = true;
                ship.GetComponent<Ship>().TakeDamage(GameManager.Instance.CurrentCannonModel.Damage);
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
}