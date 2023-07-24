using System.Collections.Generic;
using Model;

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
                name: "Base Turret",
                sprite: "Sprites/turret_1",
                clip: "Audio/turret_fire_1",
                baseSpeed: 400,
                baseRate: 600,
                baseDamage: 20,
                baseAmmo: 26,
                speedBaseCost: 100,
                rateBaseCost: 180,
                damageBaseCost: 250,
                ammoBaseCost: 125,
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
                    { 0, 2 },
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
                }
            ),
        };

        public readonly ShipModel[] Ships =
        {
            new(
                name: "SpeedBoat",
                sprite: "Sprites/ship_1",
                fireClip: null,
                explodeClip: "Audio/ship_destroy",
                baseSpeed: 130*4,
                baseRate: 0,
                baseDamage: 50,
                baseHealth: 90,
                baseMoney: 103
            ),
            new(
                name: "Vessel",
                sprite: "Sprites/ship_2",
                fireClip: "Audio/cannon_fire_1",
                explodeClip: "Audio/ship_destroy",
                baseSpeed: 100,
                baseRate: 30,
                baseDamage: 75,
                baseHealth: 180,
                baseMoney: 258
            ),
        };

        public readonly WaveModel[] Waves =
        {
            new(ships: new List<int> { 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0 })
        };
    }
}