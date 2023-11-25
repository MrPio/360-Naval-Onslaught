using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

public class DropBomb : MonoBehaviour
{
    [SerializeField] private AudioClip cannonMiss, cannonHit;
    [SerializeField] private GameObject explosion, splash;
    [SerializeField] private bool isAlly;
    [SerializeField] private float radius;

    public void End()
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
        foreach (var mainBase in Physics2D
                     .OverlapCircleAll(currentPos, radius)
                     .Where(col => col.CompareTag(isAlly ? "ship" : "base")))
        {
            hit = true;
            if (isAlly)
                mainBase.GetComponent<Ship>().TakeDamage(GameManager.Instance.SpecialDamage1);
            else
                mainBase.GetComponent<Base>().TakeDamage(DataManager.Instance.Ships[4].Damage);
        }

        MainCamera.AudioSource.PlayOneShot(hit ? cannonHit : cannonMiss,0.9f);

        Destroy(gameObject);
    }
}