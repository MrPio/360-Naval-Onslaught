using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

public class DropBomb : MonoBehaviour
{
    [SerializeField] private AudioClip cannonMiss, cannonHit;
    [SerializeField] private GameObject explosion, splash;

    public void End()
    {
        var currentPos=transform.position;

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
        foreach (var mainBase in Physics2D
                     .OverlapCircleAll(currentPos, GameManager.Instance.CurrentCannonModel.Radius / 100f)
                     .Where(col => col.CompareTag("base")))
        {
            hit = true;
            mainBase.GetComponent<Base>().TakeDamage(DataManager.Instance.Ships[4].Damage);
        }

        MainCamera.AudioSource.PlayOneShot(hit ? cannonHit : cannonMiss);

        Destroy(gameObject);
    }
}
