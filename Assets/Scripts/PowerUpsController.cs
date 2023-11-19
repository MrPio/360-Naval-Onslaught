using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Managers;
using UnityEngine;

public class PowerUpsController : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;

    [SerializeField] private GameObject missileAttack, powerUpUI;
    [SerializeField] private WaveSpawner waveSpawner;

    public void ShowRadialSlider()
    {
        powerUpUI.SetActive(true);
    }

    public void PerformAttack()
    {
        IEnumerator SpawnMissiles()
        {
            var bound = new Bounds(
                Vector3.zero,
                new Vector2(MainCamera.MainCam.GetWidth(), MainCamera.MainCam.GetHeight()) * 2f
            );
            for (var i = 0; i < Game.MissileAssaultCount; i++)
            {
                if (!Game.HasPowerUp)
                    break;
                yield return new WaitWhile(() => waveSpawner.isPaused);

                var spawnPoint = bound.GetRandomPointInBounds();
                var availableShips = GameObject.FindGameObjectsWithTag("ship")
                    .Where(it => bound.Contains(it.transform.position)).ToList();
                if (availableShips.Count > 0 && Random.Range(0f, 1f) < 0.5f)
                {
                    var ship = availableShips.RandomItem();
                    var isMoving = !ship.GetComponent<ShipPath>().IsPathEnded;
                    spawnPoint = isMoving
                        ? ship.transform.position + (ship.transform.rotation * Vector2.left * 2)
                        : ship.transform.position;
                }

                var newMissileAttack = Instantiate(
                    missileAttack,
                    spawnPoint,
                    Quaternion.identity
                );
                var missile = newMissileAttack.GetComponentInChildren<Missile>();
                missile.StartPosition = new Vector2(0.033f, 17f);
                missile.Destination = new Vector2(0.033f, 0f);
                missile.Damage = (int)(200 * (1f + 4f * Game.WaveFactor));
                yield return new WaitForSeconds(Game.PowerUpDuration / Game.MissileAssaultCount *
                                                Random.Range(0.75f, 1.15f));
            }
        }

        StartCoroutine(SpawnMissiles());
    }
}