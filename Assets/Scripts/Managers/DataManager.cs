using System.Collections.Generic;
using JetBrains.Annotations;
using Model;
using UnityEngine;

namespace Managers
{
    [System.Serializable]
    public class DataManager
    {
        private static readonly string IOName = "DataManager.json";
        private static readonly int Ship4 = Animator.StringToHash("ship_4");
        private static DataManager _instance;

        public static void Reset()
        {
            IOManager.Delete(IOName);
            _instance = new DataManager();
        }

        public void Save() => IOManager.Save(Instance, IOName);

        public static bool Load()
        {
            var instance = IOManager.Load<DataManager>(IOName);
            if (instance is not null)
                _instance = instance;
            return instance is not null;
        }

        private DataManager()
        {
        }

        public static DataManager Instance => _instance ??= new DataManager();


        public readonly TurretModel[] Turrets =
        {
            new(
                name: "Base Turret", sprite: "Sprites/my/turret_1", fireClip: "Audio/turret_fire_1",
                bulletSprite: "Sprites/bullet_1",
                baseSpeed: 380, baseRate: 300, baseDamage: 20, baseAmmo: 32, baseReload: 62,
                speedBaseCost: 90, rateBaseCost: 140, damageBaseCost: 170, ammoBaseCost: 125, reloadBaseCost: 115
            ),
            new(
                name: "Gatling Gun", sprite: "Sprites/my/turret_2", fireClip: "Audio/turret_fire_2",
                bulletSprite: "Sprites/bullet_2",
                baseSpeed: 600, baseRate: 1200, baseDamage: 12, baseAmmo: 80, baseReload: 58,
                speedBaseCost: 200, rateBaseCost: 460, damageBaseCost: 430, ammoBaseCost: 185, reloadBaseCost: 225,
                waveUnlock: 5
            ),
            new(
                name: "Missile Launcher", sprite: "Sprites/my/turret_3", fireClip: "Audio/cannon_fire_3",
                bulletSprite: "Sprites/missile_2",
                baseSpeed: 220, baseRate: 40, baseDamage: 600, baseAmmo: 2, baseReload: 42,
                speedBaseCost: 420, rateBaseCost: 630, damageBaseCost: 620, ammoBaseCost: 1500, reloadBaseCost: 475,
                waveUnlock: 10
            ),
            new(
                name: "Laser Gun", sprite: "Sprites/my/turret_4", fireClip: "Audio/laser",
                bulletSprite: "",
                baseSpeed: 1250, baseRate: 2000, baseDamage: 22, baseAmmo: 140, baseReload: 46,
                speedBaseCost: 420, rateBaseCost: 630, damageBaseCost: 650, ammoBaseCost: 780, reloadBaseCost: 475,
                waveUnlock: 15
            ),
            new(
                name: "Auto-Locking Gun", sprite: "Sprites/my/turret_5", fireClip: "Audio/turret_fire_1",
                bulletSprite: "Sprites/bullet_2",
                baseSpeed: 395, baseRate: 580, baseDamage: 74, baseAmmo: 38, baseReload: 40,
                speedBaseCost: 420, rateBaseCost: 530, damageBaseCost: 650, ammoBaseCost: 500, reloadBaseCost: 475,
                waveUnlock: 20
            ),
        };

        public readonly CannonModel[] Cannons =
        {
            new(
                name: "Base Cannon", sprite: "Sprites/my/cannon_1", fireClip: "Audio/cannon_fire_2"
                , cannonBallSprite: "Sprites/cannon_ball_1",
                baseSpeed: 42, baseDamage: 200, baseReload: 20, baseRadius: 14,
                speedBaseCost: 250, damageBaseCost: 200, reloadBaseCost: 312, radiusBaseCost: 342
            ),
            new(
                name: "Artillery", sprite: "Sprites/my/cannon_2", fireClip: "Audio/cannon_fire_1",
                cannonBallSprite: "Sprites/cannon_ball_2",
                baseSpeed: 50, baseDamage: 500, baseReload: 24, baseRadius: 16,
                speedBaseCost: 550, damageBaseCost: 540, reloadBaseCost: 615, radiusBaseCost: 520,
                waveUnlock: 5
            ),
            new(
                name: "EMP Cannon", sprite: "Sprites/my/cannon_3", fireClip: "Audio/cannon_fire_3",
                baseSpeed: 52, baseDamage: 760, baseReload: 20, baseRadius: 18,
                cannonBallSprite: "Sprites/cannon_ball_3",
                speedBaseCost: 572, damageBaseCost: 620, reloadBaseCost: 842, radiusBaseCost: 715,
                waveUnlock: 10
            ),
            new(
                name: "Blizzard Cannon", sprite: "Sprites/my/cannon_4", fireClip: "Audio/cannon_fire_1",
                baseSpeed: 55, baseDamage: 1175, baseReload: 18, baseRadius: 20,
                cannonBallSprite: "Sprites/cannon_ball_4_2",
                speedBaseCost: 572, damageBaseCost: 620, reloadBaseCost: 842, radiusBaseCost: 715,
                waveUnlock: 15
            ),
            new(
                name: "Rocket Launcher", sprite: "Sprites/my/cannon_5", fireClip: "Audio/missile_launch",
                baseSpeed: 57, baseDamage: 1000, baseReload: 18, baseRadius: 34,
                cannonBallSprite: "Sprites/cannon_ball_5_2",
                speedBaseCost: 572, damageBaseCost: 620, reloadBaseCost: 842, radiusBaseCost: 715,
                waveUnlock: 20
            ),
        };

        [System.NonSerialized] public readonly ShipModel[] Ships =
        {
            new(
                name: "SpeedBoat", sprite: "Sprites/my/ship_1", foamAnim: "foam_1", fireClip: null,
                explodeClip: "Audio/ship_destroy",
                baseSpeed: 100, baseRate: 0, baseDamage: 350, baseHealth: 90, baseMoney: 72,
                hasPath: false, missileSprite: null
            ),
            new(
                name: "Vessel", sprite: "Sprites/my/ship_2", foamAnim: "foam_2", missileSprite: "Sprites/missile_2",
                fireClip: new[] { "Audio/cannon_fire_4", "Audio/missile_coming" }, explodeClip: "Audio/ship_destroy",
                baseSpeed: 92, baseRate: 20, baseDamage: 145, baseHealth: 220, baseMoney: 118,
                hasPath: true, explosionsCount: 2
            ),
            new(
                name: "Military", sprite: "Sprites/my/ship_3", foamAnim: "foam_3", missileSprite: "Sprites/missile_3",
                fireClip: new[] { "Audio/cannon_fire_4", "Audio/missile_coming" }, explodeClip: "Audio/ship_destroy",
                baseSpeed: 70, baseRate: 10, baseDamage: 365, baseHealth: 430, baseMoney: 168,
                hasPath: true, explosionsCount: 5
            ),
            new(
                name: "Submarine", sprite: "Sprites/my/ship_4", foamAnim: "foam_4", missileSprite: "Sprites/missile_1",
                fireClip: new[] { "Audio/cannon_fire_4", "Audio/missile_coming" }, explodeClip: "Audio/ship_destroy",
                baseSpeed: 75, baseRate: 9, baseDamage: 385, baseHealth: 462, baseMoney: 274,
                hasPath: true, explosionsCount: 4,
                startCallback: go =>
                {
                    go.transform.Find("foam").gameObject.SetActive(false);
                    var renderer = go.GetComponent<Renderer>();
                    renderer.material.color = new Color(1, 1, 1, 0.5f);
                    renderer.sortingOrder = -1;
                    go.GetComponent<Ship>().Invincible = true;
                    MainCamera.AudioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/submarine_down"));
                },
                endPathCallback: go =>
                {
                    go.transform.Find("foam").gameObject.SetActive(true);
                    go.transform.Find("foam").GetComponent<Animator>().Play("foam_4");
                    go.GetComponent<Ship>().Invincible = false;
                    var overlay = go.transform.Find("overlay");
                    overlay.gameObject.SetActive(true);
                    overlay.GetComponent<Animator>().SetTrigger(Ship4);
                    overlay.GetComponent<SpriteRenderer>().sprite = go.GetComponent<SpriteRenderer>().sprite;
                    overlay.GetComponent<SpriteRenderer>().sortingOrder = 0;
                    MainCamera.AudioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/submarine_up"));
                }
            ),
            new(
                name: "Aircraft Carrier", sprite: "Sprites/my/ship_5", foamAnim: "foam_5",
                missileSprite: "Sprites/missile_1",
                fireClip: new[] { "Audio/cannon_fire_4", "Audio/missile_coming" }, explodeClip: "Audio/ship_destroy",
                baseSpeed: 55, baseRate: 6, baseDamage: 625, baseHealth: 880, baseMoney: 505, delayMultiplier: 1.5f,
                hasPath: true, explosionsCount: 9
            ),
        };

        public readonly WaveModel[] Waves =
        {
            // new(shipsChances: new List<float> { 0f, 0.1f, 0.1f, 1f, 1f },shipsCount:71),

            new(shipsChances: new List<float> { 0.75f, 1f, 1f, 1f, 1f }, shipsCount: 7),
            new(shipsChances: new List<float> { 0.65f, 0.9f, 1f, 1f, 1f }, shipsCount: 9),
            new(shipsChances: new List<float> { 0.5f, 0.8f, 1f, 1f, 1f }, shipsCount: 12),
            new(shipsChances: new List<float> { 1f, 1f, 1f, 1f, 1f }, shipsCount: 14, spawnSpeedMultiply: 1.8f),
            new(shipsChances: new List<float> { 0.35f, 0.5f, 0.885f, 1f, 1f }, shipsCount: 15,
                spawnSpeedMultiply: 0.85f),

            new(shipsChances: new List<float> { 0.55f, 0.7f, 0.8f, 0.94f, 1f }, shipsCount: 12),
            new(shipsChances: new List<float> { 1f, 1f, 1f, 1f, 1f }, shipsCount: 22, spawnSpeedMultiply: 2f),
            new(shipsChances: new List<float> { 0.45f, 0.6f, 0.86f, 0.94f, 1f }, shipsCount: 15),
            new(shipsChances: new List<float> { 0.35f, 0.6f, 0.87f, 0.94f, 1f }, shipsCount: 14),
            new(shipsChances: new List<float> { 0.35f, 0.6f, 0.87f, 0.94f, 1f }, shipsCount: 20,
                spawnSpeedMultiply: 0.8f),

            new(shipsChances: new List<float> { 0.15f, 0.3f, 0.85f, 0.92f, 1f }, shipsCount: 14),
            new(shipsChances: new List<float> { 1f, 1f, 1f, 1f, 1f }, shipsCount: 24, spawnSpeedMultiply: 2.55f),
            new(shipsChances: new List<float> { 0.35f, 0.4f, 0.87f, 0.96f, 1f }, shipsCount: 16),
            new(shipsChances: new List<float> { 0.1f, 0.2f, 0.86f, 0.94f, 1f }, shipsCount: 18),
            new(shipsChances: new List<float> { 0.35f, 0.6f, 0.87f, 0.94f, 1f }, shipsCount: 20,
                spawnSpeedMultiply: 0.8f),

            new(shipsChances: new List<float> { 0.15f, 0.3f, 0.65f, 0.96f, 1f }, shipsCount: 17),
            new(shipsChances: new List<float> { 1f, 1f, 1f, 1f, 1f }, shipsCount: 16, spawnSpeedMultiply: 2.5f),
            new(shipsChances: new List<float> { 0.35f, 0.4f, 0.75f, 0.94f, 1f }, shipsCount: 22),
            new(shipsChances: new List<float> { 0.1f, 0.2f, 0.85f, 0.64f, 1f }, shipsCount: 17),
            new(shipsChances: new List<float> { 0.15f, 0.25f, 0.785f, 0.95f, 1f }, shipsCount: 24,
                spawnSpeedMultiply: 0.8f),

            new(shipsChances: new List<float> { 0.25f, 0.38f, 0.885f, 0.955f, 1f }, shipsCount: 11),
            new(shipsChances: new List<float> { 0.25f, 0.38f, 0.885f, 0.945f, 1f }, shipsCount: 16),
            new(shipsChances: new List<float> { 0.35f, 0.4f, 0.85f, 0.94f, 1f }, shipsCount: 22),
            new(shipsChances: new List<float> { 0.1f, 0.2f, 0.85f, 0.92f, 1f }, shipsCount: 26),
            new(shipsChances: new List<float> { 0.25f, 0.45f, 0.75f, 0.92f, 1f }, shipsCount: 50,
                spawnSpeedMultiply: 0.825f),
        };
    }
}