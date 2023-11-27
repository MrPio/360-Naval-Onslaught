using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ExtensionsFunctions;
using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class CannonBall : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;

    public float Duration = 1;
    [SerializeField] private AudioClip cannonMiss, cannonHit;
    [SerializeField] private GameObject explosion, splash;
    [SerializeField] private List<Transform> smallCannonBallSpawnPoints;
    [SerializeField] private GameObject smallCannonBall;
    private float _accumulator;
    [NonSerialized] public Vector2 Destination, StartPos;
    [NonSerialized] public bool SmallCannonBall, hasEMP;

    private void Update()
    {
        _accumulator += Time.deltaTime;
        var currentPos = Vector2.Lerp(StartPos, Destination, _accumulator / Duration);
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

            if (SmallCannonBall)
                smallCannonBallSpawnPoints.ForEach(point =>
                {
                    Instantiate(
                        original: smallCannonBall,
                        position: point.position,
                        rotation: Quaternion.identity
                    ).transform.rotation=point.localPosition.ToQuaternion();
                });

            var hit = false;
            // Check collisions
            foreach (var ship in Physics2D
                         .OverlapCircleAll(currentPos, Game.CurrentCannonModel.Radius / 100f)
                         .Where(col => col.CompareTag("ship") || col.CompareTag("power_up") )
                    )
            {
                hit = true;
                ++Game.CurrentWaveCannonHit;
                ship.GetComponent<IDamageable>().TakeDamage(Game.CurrentCannonModel.Damage,critical:Game.IsCannonCritical,emp:hasEMP);
            }
            

            MainCamera.AudioSource.PlayOneShot(hit ? cannonHit : cannonMiss);
            if (!Game.IsSpecialWave && hit)
                GameObject.FindWithTag("camera_container").GetComponent<Animator>().SetTrigger("one_shake");


            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Game.CurrentCannonModel.Radius / 100f);
    }
}