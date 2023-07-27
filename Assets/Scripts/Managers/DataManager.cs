using System.Collections.Generic;
using Model;
using UnityEngine;

namespace Managers
{
    public class DataManager
    {
        private static DataManager _instance;

        private DataManager()
        {
        }

        public static DataManager Instance => _instance ??= new DataManager();


        public readonly TurretModel[] Turrets =
        {
            new(
                name: "Base Turret", sprite: "Sprites/turret_1", fireClip: "Audio/turret_fire_1",
                bulletSprite: "Sprites/bullet_1",
                baseSpeed: 400 * 5, baseRate: 300 * 8, baseDamage: 20, baseAmmo: 26 * 2, baseReload: 75 * 3,
                speedBaseCost: 100, rateBaseCost: 180, damageBaseCost: 150, ammoBaseCost: 125, reloadBaseCost: 115,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 20 },
                    { 5, 25 },
                    { 10, 35 },
                    { 15, 50 },
                    { 20, 70 },
                    { 25, 85 },
                    { 30, 100 },
                    { 35, 125 },
                },
                rateLevelSteps: new Dictionary<int, int>
                {
                    { 0, 60 },
                    { 5, 70 },
                    { 10, 80 },
                    { 15, 100 },
                    { 20, 120 },
                    { 25, 150 },
                    { 30, 180 },
                    { 35, 220 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 1 },
                    { 5, 2 },
                    { 10, 3 },
                    { 15, 3 },
                    { 20, 4 },
                    { 25, 4 },
                    { 30, 6 },
                    { 35, 6 },
                },
                ammoLevelSteps: new Dictionary<int, int>
                {
                    { 0, 2 },
                    { 5, 2 },
                    { 10, 3 },
                    { 15, 3 },
                    { 20, 4 },
                    { 25, 4 },
                    { 30, 6 },
                    { 35, 6 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 25 },
                    { 5, 30 },
                    { 10, 35 },
                    { 15, 40 },
                    { 20, 50 },
                    { 25, 65 },
                    { 30, 70 },
                    { 35, 80 },
                }
            ),
            new(
                name: "Gatling Gun", sprite: "Sprites/turret_2", fireClip: "Audio/turret_fire_2",
                bulletSprite: "Sprites/bullet_2",
                baseSpeed: 500, baseRate: 800, baseDamage: 26, baseAmmo: 46, baseReload: 115,
                speedBaseCost: 200, rateBaseCost: 260, damageBaseCost: 360, ammoBaseCost: 185, reloadBaseCost: 225,
                waveUnlock:5,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 20 },
                    { 5, 25 },
                    { 10, 35 },
                    { 15, 50 },
                    { 20, 70 },
                    { 25, 85 },
                    { 30, 100 },
                    { 35, 125 },
                },
                rateLevelSteps: new Dictionary<int, int>
                {
                    { 0, 60 },
                    { 5, 70 },
                    { 10, 80 },
                    { 15, 100 },
                    { 20, 120 },
                    { 25, 150 },
                    { 30, 180 },
                    { 35, 220 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 1 },
                    { 3, 2 },
                    { 10, 3 },
                    { 15, 3 },
                    { 20, 4 },
                    { 25, 4 },
                    { 30, 6 },
                    { 35, 6 },
                },
                ammoLevelSteps: new Dictionary<int, int>
                {
                    { 0, 4 },
                    { 5, 4 },
                    { 10, 5 },
                    { 15, 5 },
                    { 20, 5 },
                    { 25, 6 },
                    { 30, 6 },
                    { 35, 6 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 25 },
                    { 5, 30 },
                    { 10, 35 },
                    { 15, 40 },
                    { 20, 50 },
                    { 25, 65 },
                    { 30, 70 },
                    { 35, 80 },
                }
            ),
            new(
                name: "Missile Launcher", sprite: "Sprites/turret_3", fireClip: "Audio/cannon_fire_3",
                bulletSprite: "Sprites/missile_2",
                baseSpeed: 200, baseRate: 40, baseDamage: 200, baseAmmo: 2, baseReload: 45,
                speedBaseCost: 420, rateBaseCost: 430, damageBaseCost: 550, ammoBaseCost: 780, reloadBaseCost: 475,
                waveUnlock:10,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 20 },
                    { 5, 25 },
                    { 10, 35 },
                    { 15, 50 },
                    { 20, 70 },
                    { 25, 85 },
                    { 30, 100 },
                    { 35, 125 },
                },
                rateLevelSteps: new Dictionary<int, int>
                {
                    { 0, 4 },
                    { 5, 6 },
                    { 10, 8 },
                    { 15, 10 },
                    { 20, 12 },
                    { 25, 14 },
                    { 30, 16 },
                    { 35, 18 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 10 },
                    { 5, 12 },
                    { 10, 14 },
                    { 15, 16 },
                    { 20, 18 },
                    { 25, 20 },
                    { 30, 24 },
                    { 35, 28 },
                },
                ammoLevelSteps: new Dictionary<int, int>
                {
                    { 0, 1 },
                    { 5, 1 },
                    { 10, 2 },
                    { 15, 2 },
                    { 20, 2 },
                    { 25, 4 },
                    { 30, 4 },
                    { 35, 4 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 8 },
                    { 5, 12 },
                    { 10, 16 },
                    { 15, 20 },
                    { 20, 30 },
                    { 25, 35 },
                    { 30, 40 },
                    { 35, 50 },
                }
            ),
                        new(
                name: "Piercing Gun", sprite: "Sprites/turret_4", fireClip: "Audio/turret_fire_2",
                bulletSprite: "Sprites/bullet_2",
                baseSpeed: 1250, baseRate: 2000, baseDamage: 22, baseAmmo: 77, baseReload: 46,
                speedBaseCost: 420, rateBaseCost: 430, damageBaseCost: 550, ammoBaseCost: 780, reloadBaseCost: 475,
                waveUnlock:15,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 20 },
                    { 5, 25 },
                    { 10, 35 },
                    { 15, 50 },
                    { 20, 70 },
                    { 25, 85 },
                    { 30, 100 },
                    { 35, 125 },
                },
                rateLevelSteps: new Dictionary<int, int>
                {
                    { 0, 4 },
                    { 5, 6 },
                    { 10, 8 },
                    { 15, 10 },
                    { 20, 12 },
                    { 25, 14 },
                    { 30, 16 },
                    { 35, 18 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 10 },
                    { 5, 12 },
                    { 10, 14 },
                    { 15, 16 },
                    { 20, 18 },
                    { 25, 20 },
                    { 30, 24 },
                    { 35, 28 },
                },
                ammoLevelSteps: new Dictionary<int, int>
                {
                    { 0, 1 },
                    { 5, 1 },
                    { 10, 2 },
                    { 15, 2 },
                    { 20, 2 },
                    { 25, 4 },
                    { 30, 4 },
                    { 35, 4 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 8 },
                    { 5, 12 },
                    { 10, 16 },
                    { 15, 20 },
                    { 20, 30 },
                    { 25, 35 },
                    { 30, 40 },
                    { 35, 50 },
                }
            ),
                                    new(
                name: "Auto-Locking Gun", sprite: "Sprites/turret_5", fireClip: "Audio/turret_fire_1",
                bulletSprite: "Sprites/bullet_2",
                baseSpeed: 365, baseRate: 480, baseDamage: 70, baseAmmo: 19, baseReload: 37,
                speedBaseCost: 420, rateBaseCost: 430, damageBaseCost: 550, ammoBaseCost: 780, reloadBaseCost: 475,
                waveUnlock:20,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 20 },
                    { 5, 25 },
                    { 10, 35 },
                    { 15, 50 },
                    { 20, 70 },
                    { 25, 85 },
                    { 30, 100 },
                    { 35, 125 },
                },
                rateLevelSteps: new Dictionary<int, int>
                {
                    { 0, 4 },
                    { 5, 6 },
                    { 10, 8 },
                    { 15, 10 },
                    { 20, 12 },
                    { 25, 14 },
                    { 30, 16 },
                    { 35, 18 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 10 },
                    { 5, 12 },
                    { 10, 14 },
                    { 15, 16 },
                    { 20, 18 },
                    { 25, 20 },
                    { 30, 24 },
                    { 35, 28 },
                },
                ammoLevelSteps: new Dictionary<int, int>
                {
                    { 0, 1 },
                    { 5, 1 },
                    { 10, 2 },
                    { 15, 2 },
                    { 20, 2 },
                    { 25, 4 },
                    { 30, 4 },
                    { 35, 4 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 8 },
                    { 5, 12 },
                    { 10, 16 },
                    { 15, 20 },
                    { 20, 30 },
                    { 25, 35 },
                    { 30, 40 },
                    { 35, 50 },
                }
            ),
        };

        public readonly CannonModel[] Cannons =
        {
            new(
                name: "Base Cannon", sprite: "Sprites/cannon_1", fireClip: "Audio/cannon_fire_2"
                ,cannonBallSprite: "Sprites/cannon_ball_1",
                baseSpeed: 50, baseDamage: 200, baseReload: 20 * 5, baseRadius: 20,
                speedBaseCost: 450, damageBaseCost: 300, reloadBaseCost: 350, radiusBaseCost: 250,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 15 },
                    { 5, 20 },
                    { 10, 25 },
                    { 15, 30 },
                    { 20, 40 },
                    { 25, 55 },
                    { 30, 60 },
                    { 35, 75 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 10 },
                    { 5, 16 },
                    { 10, 24 },
                    { 15, 32 },
                    { 20, 40 },
                    { 25, 50 },
                    { 30, 65 },
                    { 35, 75 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 2 },
                    { 5, 3 },
                    { 10, 4 },
                    { 15, 6 },
                    { 20, 8 },
                    { 25, 10 },
                    { 30, 12 },
                    { 35, 15 },
                },
                radiusLevelSteps: new Dictionary<int, int>
                {
                    { 0, 2 },
                    { 15, 3 },
                    { 30, 4 },
                }
            ),
            new(
                name: "Artillery", sprite: "Sprites/cannon_2", fireClip: "Audio/cannon_fire_1",
                cannonBallSprite: "Sprites/cannon_ball_2",
                baseSpeed: 65, baseDamage: 500, baseReload: 24, baseRadius: 22,
                speedBaseCost: 550, damageBaseCost: 540, reloadBaseCost: 615, radiusBaseCost: 520,
                waveUnlock: 5,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 15 },
                    { 5, 20 },
                    { 10, 25 },
                    { 15, 30 },
                    { 20, 40 },
                    { 25, 55 },
                    { 30, 60 },
                    { 35, 75 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 20 },
                    { 5, 26 },
                    { 10, 34 },
                    { 15, 42 },
                    { 20, 60 },
                    { 25, 70 },
                    { 30, 75 },
                    { 35, 75 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 2 },
                    { 5, 3 },
                    { 10, 4 },
                    { 15, 6 },
                    { 20, 8 },
                    { 25, 10 },
                    { 30, 12 },
                    { 35, 15 },
                },
                radiusLevelSteps: new Dictionary<int, int>
                {
                    { 0, 2 },
                    { 15, 3 },
                    { 30, 4 },
                }
            ),
            new(
                name: "EMP Cannon", sprite: "Sprites/cannon_3", fireClip: "Audio/cannon_fire_3",
                baseSpeed: 85, baseDamage: 760, baseReload: 18, baseRadius: 18,
                cannonBallSprite: "Sprites/cannon_ball_3",
                speedBaseCost: 572, damageBaseCost: 620, reloadBaseCost: 842, radiusBaseCost: 715,
                waveUnlock:10,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 15 },
                    { 5, 20 },
                    { 10, 25 },
                    { 15, 30 },
                    { 20, 40 },
                    { 25, 55 },
                    { 30, 60 },
                    { 35, 75 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 40 },
                    { 5, 46 },
                    { 10, 54 },
                    { 15, 52 },
                    { 20, 60 },
                    { 25, 70 },
                    { 30, 75 },
                    { 35, 75 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 3 },
                    { 3, 4 },
                    { 10, 6 },
                    { 15, 6 },
                    { 20, 8 },
                    { 25, 10 },
                    { 30, 12 },
                    { 35, 15 },
                },
                radiusLevelSteps: new Dictionary<int, int>
                {
                    { 0, 3 },
                    { 15, 4 },
                    { 30, 5 },
                }
            ),
            new(
                name: "Blizzard Cannon", sprite: "Sprites/cannon_4", fireClip: "Audio/cannon_fire_1",
                baseSpeed: 95, baseDamage: 1175, baseReload: 18, baseRadius: 22,
                cannonBallSprite: "Sprites/cannon_ball_4",
                speedBaseCost: 572, damageBaseCost: 620, reloadBaseCost: 842, radiusBaseCost: 715,
                waveUnlock:15,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 15 },
                    { 5, 20 },
                    { 10, 25 },
                    { 15, 30 },
                    { 20, 40 },
                    { 25, 55 },
                    { 30, 60 },
                    { 35, 75 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 40 },
                    { 5, 46 },
                    { 10, 54 },
                    { 15, 52 },
                    { 20, 60 },
                    { 25, 70 },
                    { 30, 75 },
                    { 35, 75 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 3 },
                    { 3, 4 },
                    { 10, 6 },
                    { 15, 6 },
                    { 20, 8 },
                    { 25, 10 },
                    { 30, 12 },
                    { 35, 15 },
                },
                radiusLevelSteps: new Dictionary<int, int>
                {
                    { 0, 3 },
                    { 15, 4 },
                    { 30, 5 },
                }
            ),
            new(
                name: "Rocket Launcher", sprite: "Sprites/cannon_5", fireClip: "Audio/cannon_fire_1",
                baseSpeed: 115, baseDamage: 2000, baseReload: 20, baseRadius: 28,
                cannonBallSprite: "Sprites/cannon_ball_5",
                speedBaseCost: 572, damageBaseCost: 620, reloadBaseCost: 842, radiusBaseCost: 715,
                waveUnlock:20,
                speedLevelSteps: new Dictionary<int, int>
                {
                    { 0, 15 },
                    { 5, 20 },
                    { 10, 25 },
                    { 15, 30 },
                    { 20, 40 },
                    { 25, 55 },
                    { 30, 60 },
                    { 35, 75 },
                },
                damageLevelSteps: new Dictionary<int, int>
                {
                    { 0, 40 },
                    { 5, 46 },
                    { 10, 54 },
                    { 15, 52 },
                    { 20, 60 },
                    { 25, 70 },
                    { 30, 75 },
                    { 35, 75 },
                },
                reloadLevelSteps: new Dictionary<int, int>
                {
                    { 0, 3 },
                    { 3, 4 },
                    { 10, 6 },
                    { 15, 6 },
                    { 20, 8 },
                    { 25, 10 },
                    { 30, 12 },
                    { 35, 15 },
                },
                radiusLevelSteps: new Dictionary<int, int>
                {
                    { 0, 3 },
                    { 15, 4 },
                    { 30, 5 },
                }
            ),
        };

        public readonly ShipModel[] Ships =
        {
            new(
                name: "SpeedBoat", sprite: "Sprites/ship_1", fireClip: null, explodeClip: "Audio/ship_destroy",
                baseSpeed: 130 * 1, baseRate: 0, baseDamage: 50, baseHealth: 90, baseMoney: 103,
                hasPath: false, missileSprite: null
            ),
            new(
                name: "Vessel", sprite: "Sprites/ship_2", missileSprite: "Sprites/missile_2",
                fireClip: new[] { "Audio/cannon_fire_4", "Audio/missile_coming" }, explodeClip: "Audio/ship_destroy",
                baseSpeed: 92, baseRate: 12, baseDamage: 75, baseHealth: 220, baseMoney: 258,
                hasPath: true, explosionsCount: 2
            ),
            new(
                name: "Military", sprite: "Sprites/ship_3", missileSprite: "Sprites/missile_3",
                fireClip: new[] { "Audio/cannon_fire_4", "Audio/missile_coming" }, explodeClip: "Audio/ship_destroy",
                baseSpeed: 70, baseRate: 20, baseDamage: 185, baseHealth: 460, baseMoney: 681,
                hasPath: true, explosionsCount: 5
            ),
            new(
                name: "Submarine", sprite: "Sprites/ship_4", missileSprite: "Sprites/missile_1",
                fireClip: new[] { "Audio/cannon_fire_4", "Audio/missile_coming" }, explodeClip: "Audio/ship_destroy",
                baseSpeed: 175, baseRate: 13, baseDamage: 525, baseHealth: 782, baseMoney: 1460,
                hasPath: true, explosionsCount: 4,
                startCallback: go =>
                {
                    var renderer = go.GetComponent<Renderer>();
                    renderer.material.color = new Color(1, 1, 1, 0.5f);
                    go.GetComponent<Ship>().Invincible = true;
                    MainCamera.AudioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/submarine_down"));
                },
                endPathCallback: go =>
                {
                    go.GetComponent<Ship>().Invincible = false;
                    var overlay = go.transform.Find("overlay");
                    overlay.gameObject.SetActive(true);
                    overlay.GetComponent<Animator>().SetTrigger(Ship4);
                    overlay.GetComponent<SpriteRenderer>().sprite = go.GetComponent<SpriteRenderer>().sprite;
                    MainCamera.AudioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/submarine_up"));
                }
            ),
        };

        public readonly WaveModel[] Waves =
        {
            new(ships: new List<int> { 0, 0, 0, 3, 3, 2, 1, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),

            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),

            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),

            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 }),
        };

        private static readonly int Ship4 = Animator.StringToHash("ship_4");
    }
}