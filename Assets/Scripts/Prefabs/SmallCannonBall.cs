using System.Linq;
using Managers;
using UnityEngine;

public class SmallCannonBall : MonoBehaviour
{
    [SerializeField] private GameObject explosion, splash;
    [SerializeField] private AudioClip cannonMiss, cannonHit;


    void EndAnimation()
    {
        var currentPos = transform.position;
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
                     .Where(col => new[] { "ship", "power_up" }.Any(col.CompareTag))
                )
        {
            hit = true;
            ship.GetComponent<Ship>().TakeDamage(GameManager.Instance.CurrentCannonModel.Damage/4);
        }

        MainCamera.AudioSource.PlayOneShot(hit ? cannonHit : cannonMiss,0.5f);

        Destroy(gameObject);
    }
}
