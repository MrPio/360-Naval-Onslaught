﻿using System.Collections.Generic;
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
                fireClip: "Audio/turret_fire_1",
                baseSpeed: 400 * 5,
                baseRate: 300 * 8,
                baseDamage: 20,
                baseAmmo: 26 * 2,
                baseReload: 75 * 3,
                speedBaseCost: 100,
                rateBaseCost: 180,
                damageBaseCost: 150,
                ammoBaseCost: 125,
                reloadBaseCost: 115,
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
        };

        public readonly CannonModel[] Cannons =
        {
            new(
                name: "Base Cannon",
                sprite: "Sprites/cannon_1",
                fireClip: "Audio/cannon_fire_1",
                baseSpeed: 100,
                baseDamage: 200,
                baseReload: 20,
                speedBaseCost: 450,
                damageBaseCost: 300,
                reloadBaseCost: 350,
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
                baseSpeed: 130 * 1,
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
            new(ships: new List<int>
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0
            }),
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
    }
}