using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using JetBrains.Annotations;
using Model;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    [Serializable]
    public class GameManager
    {
        private static readonly string IOName = "GameManager.json";

        private static DataManager Data => DataManager.Instance;

        public static void Reset()
        {
            DeleteSave();
            _instance = new GameManager();
        }

        public static void DeleteSave() => IOManager.Delete(IOName);


        public void Save() => IOManager.Save(Instance, IOName);

        public static bool Load()
        {
            var instance = IOManager.Load<GameManager>(IOName);
            if (instance is not null)
                _instance = instance;
            return instance is not null;
        }

        public static bool HasSave() => IOManager.Exist(IOName);


        private static GameManager _instance;

        private GameManager()
        {
            CannonAmmo = 1;
            Health = MaxHealth;
        }

        public static GameManager Instance => _instance ??= new GameManager();

        public int Quality = InputManager.IsMobile ? 0 : 2;
        public readonly string[] QualityNames = { "Low", "High", "Ultra" };
        public int Difficulty = InputManager.IsMobile ? 0 : 1;
        public readonly string[] DifficultyNames = { "Easy", "Medium", "Hard" };

        public string[] QualityOceanMaterials =
            { "Materials/ocean_low", "Materials/ocean_high", "Materials/ocean_ultra" };

        public int Health;
        public float Armor = 0.5f;
        public int MaxHealth = 3500;
        private readonly int _healthBaseStep = 175;
        private readonly int _healthBaseCost = 400;
        public int HealthLevel;
        [SerializeField] private int _repairLevel;
        public int Wave = 0;
        [NonSerialized] public int SpecialWave = -1;
        public int Ammo;
        [NonSerialized] public int CannonAmmo = 1;
        public int Money = 200;
        public int Diamonds = 0, TotalDiamonds, PendingDiamonds;
        public int CurrentTurret = 0;
        public int CurrentCannon = 0;
        public int Score;
        public float SpecialShipChance = 0.05f;

        public float DiamondHealth => (100f + 300f * WaveFactor) *
                                      (Difficulty == 0 ? 0.8f : 1) *
                                      (Difficulty == 2 ? 1.5f : 1);

        public float DiamondLifespan => 7f *
                                        (Difficulty == 0 ? 1.15f : 1) *
                                        (Difficulty == 2 ? 0.87f : 1);

        [NonSerialized] public bool HasOverride;
        [NonSerialized] public float OverrideAmount;
        public float OverheatingDurationFactor => IsSpecialWave ? 0.5f : 1.5f;
        [NonSerialized] public bool IsOverheated;

        public float OverrideDuration => 2.5f *
                                         (IsSpecialWave ? 2.5f : 1f) *
                                         (Difficulty == 0 ? 1.15f : 1) *
                                         (Difficulty == 2 ? 0.9f : 1);

        public bool DrawArmoredShip => new System.Random().Next(0, 100) < 10;

        public bool CanSpawnDiamond => Wave / (Data.Waves.Length / (Data.PowerUps.Length - 1f)) >=
                                       TotalDiamonds + PendingDiamonds - 1;

        public bool DrawDiamond => new System.Random().Next(0, 100) < 50;


        // === CRITICAL HIT =========================================================
        private readonly int _criticalFactorBaseCost = 325,
            _turretCriticalChanceBaseCost = 400,
            _cannonCriticalChanceBaseCost = 300;

        public float CriticalFactor => 2f + Enumerable.Range(0, CriticalFactorLevel).Sum(CriticalFactorStep);
        public float TurretCriticalChance => 0.03f + TurretCriticalChanceStep * TurretCriticalChanceLevel;
        public float CannonCriticalChance => 0.075f + CannonCriticalChanceStep * CannonCriticalChanceLevel;

        public int CriticalFactorLevel, TurretCriticalChanceLevel, CannonCriticalChanceLevel;
        public int CriticalMaxLevel = 9;

        public bool IsTurretCritical => new System.Random().Next(0, 1000) <
                                        TurretCriticalChance * 1000 * PowerUpFactor(PowerUpModel.PowerUp.Critical);

        public bool IsCannonCritical => new System.Random().Next(0, 1000) <
                                        CannonCriticalChance * 1000 * PowerUpFactor(PowerUpModel.PowerUp.Critical);

        public float CriticalFactorStep(int level = -1) => 0.25f + .15f * (level < 0 ? CriticalFactorLevel : level);
        public float TurretCriticalChanceStep => 0.015f;
        public float CannonCriticalChanceStep => 0.015f;

        public int CriticalFactorCost => (int)(_criticalFactorBaseCost * (1f + 0.75f * CriticalFactorLevel) *
                                               (Difficulty == 0 ? 0.9f : 1) *
                                               (Difficulty == 2 ? 1.25f : 1)) *
                                         (CriticalFactorLevel >= CriticalMaxLevel ? 0 : 1);

        public int TurretCriticalChanceCost =>
            (int)(_turretCriticalChanceBaseCost * (1f * (1 + CannonCriticalChanceLevel)) *
                  (Difficulty == 0 ? 0.9f : 1) *
                  (Difficulty == 2 ? 1.25f : 1)) *
            (TurretCriticalChanceLevel >= CriticalMaxLevel ? 0 : 1);

        public int CannonCriticalChanceCost =>
            (int)(_cannonCriticalChanceBaseCost * (1f * (1 + CannonCriticalChanceLevel)) *
                  (Difficulty == 0 ? 0.9f : 1) *
                  (Difficulty == 2 ? 1.25f : 1)) *
            (CannonCriticalChanceLevel >= CriticalMaxLevel ? 0 : 1);


        private void BuyCritical(ref int level, int cost)
        {
            if (Money < cost || level >= CriticalMaxLevel) return;
            Money -= cost;
            ++level;
        }

        public void BuyCriticalFactor() =>
            BuyCritical(ref CriticalFactorLevel, CriticalFactorCost);

        public void BuyTurretCriticalChance() =>
            BuyCritical(ref TurretCriticalChanceLevel, TurretCriticalChanceCost);

        public void BuyCannonCriticalChance() =>
            BuyCritical(ref CannonCriticalChanceLevel, CannonCriticalChanceCost);
        // ==========================================================================


        // === POWER-UP ======================================================
        [NonSerialized] private PowerUpModel? _powerUp;

        [CanBeNull]
        public PowerUpModel DrawPowerUp() => Data.PowerUps.Where(it => !it.IsLocked).ToList()
            .Select(it => it.Count > 0 ? it.RandomItem() : null);

        public float PowerUpSpawnChance => 0.125f + PowerUpSpawnChanceStep * PowerUpSpawnChanceLevel;
        private readonly int _powerUpSpawnChanceBaseCost = 350;

        public bool DrawPowerUpSpawn => new System.Random().Next(0, 1000) < PowerUpSpawnChance * 1000;
        public int PowerUpSpawnChanceLevel, PowerUpSpawnChanceMaxLevel = 9;

        public int PowerUpSpawnChanceCost =>
            (int)(_powerUpSpawnChanceBaseCost * (1f + 0.5f * PowerUpSpawnChanceLevel) *
                  (Difficulty == 0 ? 0.9f : 1) *
                  (Difficulty == 2 ? 1.15f : 1)) *
            (PowerUpSpawnChanceLevel >= PowerUpSpawnChanceMaxLevel ? 0 : 1);

        public float PowerUpSpawnChanceStep => 0.025f;

        public void BuyPowerUpSpawnChance()
        {
            if (Money < PowerUpSpawnChanceCost || PowerUpSpawnChanceLevel >= PowerUpSpawnChanceMaxLevel) return;
            Money -= PowerUpSpawnChanceCost;
            ++PowerUpSpawnChanceLevel;
        }

        [NonSerialized] public float PowerUpStart;
        public float PowerUpProgress => _powerUp is null ? 999f : (Time.time - PowerUpStart) / _powerUp.Duration;

        public PowerUpModel? PowerUp
        {
            get
            {
                if (PowerUpProgress >= 1)
                    _powerUp = null;
                return _powerUp;
            }
            set
            {
                if (value is null)
                {
                    PowerUpStart = 0f;
                    return;
                }

                _powerUp = value;
                PowerUpStart = Time.time;
            }
        }

        public float PowerUpFactor(PowerUpModel.PowerUp type) =>
            PowerUp?.Type == type ? PowerUp.Strength : 1f;

        // ==========================================================================

        // === FORTUNE WHEEL ========================================================
        public int WheelsSpinned;
        // ==========================================================================

        public int HealthStep => (int)(_healthBaseStep * (1f + 0.25f * HealthLevel));

        public int HealthCost => (int)(_healthBaseCost * (1f + 0.5f * HealthLevel) *
                                       (Difficulty == 0 ? 0.9f : 1) *
                                       (Difficulty == 2 ? 1.25f : 1));

        public int RepairCost => (int)(0.28f * (MaxHealth - Health) * (1f + 0.25f * _repairLevel));

        public int CurrentWaveTurretFired = 0,
            CurrentWaveTurretHit = 0,
            CurrentWaveCannonFired = 0,
            CurrentWaveCannonHit = 0;

        public float CurrentWaveTurretAccuracy => CurrentWaveTurretHit / (float)CurrentWaveTurretFired;
        public float CurrentWaveCannonAccuracy => CurrentWaveCannonHit / (float)CurrentWaveCannonFired;
        public bool HasTurretAccuracyBonus => CurrentWaveTurretAccuracy >= (InputManager.IsMobile ? 0.67f : 0.75f);
        public bool HasCannonAccuracyBonus => CurrentWaveCannonAccuracy >= 0.75;
        public bool HasAccuracyBonus => HasTurretAccuracyBonus && HasCannonAccuracyBonus;
        public bool IsSpecialWave => SpecialWave >= 0;
        public bool[] SpecialOccurInWave = new bool[99];
        public bool HasPowerUp => PowerUp is { };


        public TurretModel CurrentTurretModel => Data.Turrets[CurrentTurret];
        public CannonModel CurrentCannonModel => Data.Cannons[CurrentCannon];
        public WaveModel CurrentWave => Data.Waves[Wave];
        public float WaveFactor => Wave / (float)Data.Waves.Length;
        public List<int> SpecialsCount = new() { 1, 1, 1, 1 };
        [SerializeField] private List<int> _specialsBought = new() { 0, 0, 0, 0 };

        private List<int> SpecialsBaseCosts => Difficulty switch
        {
            0 => new List<int> { 650, 1000, 1250, 1500 },
            1 => new List<int> { 800, 1250, 1500, 1750 },
            _ => new List<int> { 1000, 1500, 1750, 2000 }
        };

        public List<string> SpecialsName = new() { "Air Assault", "Shield", "EMP", "Health" };
        public int SpecialDamage => (int)(750 * (1f + 0.1f * Wave));

        public int SpecialCost(int index) =>
            (int)(SpecialsBaseCosts[index] * Mathf.Pow(1.06f, _specialsBought[index]));


        public void BuySpecial(int index)
        {
            if (Money >= SpecialCost(index))
            {
                Money -= SpecialCost(index);
                ++_specialsBought[index];
                ++SpecialsCount[index];
            }
        }

        public void BuyHealth()
        {
            if (Money >= HealthCost)
            {
                Money -= HealthCost;
                MaxHealth += HealthStep;
                Health += HealthStep;
                HealthLevel++;
            }
        }

        public void BuyRepair()
        {
            if (Money >= RepairCost && Health < MaxHealth)
            {
                Money -= RepairCost;
                Health = MaxHealth;
                _repairLevel++;
            }
        }
    }
}