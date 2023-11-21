using System.Collections.Generic;
using JetBrains.Annotations;
using Model;
using UnityEngine;

namespace Managers
{
    [System.Serializable]
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

        public string[] QualityOceanMaterials =
            { "Materials/ocean_low", "Materials/ocean_high", "Materials/ocean_ultra" };

        public int Health;
        public int MaxHealth = 3500;
        private int _healthBaseStep = 175;
        private int _healthBaseCost = 400;
        private int _repairLevel = 1;
        public int HealthLevel = 1;
        public int Wave = 0;
        [System.NonSerialized] public int SpecialWave = -1;
        public int Ammo, CannonAmmo;
        public int Money = 200;
        public int CurrentTurret = 0;
        public int CurrentCannon = 0;
        public int Score;
        public float PowerUpDuration = 20;
        public int MissileAssaultCount = 20;
        [System.NonSerialized] private Bubble.PowerUp? _powerUp;
        [System.NonSerialized] public bool HasOverride;

        public Bubble.PowerUp? PowerUp
        {
            get
            {
                if (PowerUpProgress >= 1)
                    _powerUp = null;
                return _powerUp;
            }
            set
            {
                if (value is { })
                {
                    _powerUp = (Bubble.PowerUp)value;
                    PowerUpStart = Time.time;
                }
            }
        }

        [System.NonSerialized] public float PowerUpStart;
        public float PowerUpProgress => (Time.time - PowerUpStart) / PowerUpDuration;

        public int HealthStep => (int)(_healthBaseStep * (1f + 0.25f * HealthLevel));
        public int HealthCost => (int)(_healthBaseCost * (1f + 0.35f * HealthLevel));
        public int RepairCost => (int)(0.28f * (MaxHealth - Health) * (1f + 0.25f * _repairLevel));

        public int CurrentWaveTurretFired = 0,
            CurrentWaveTurretHit = 0,
            CurrentWaveCannonFired = 0,
            CurrentWaveCannonHit = 0;

        public float CurrentWaveTurretAccuracy => CurrentWaveTurretHit / (float)CurrentWaveTurretFired;
        public float CurrentWaveCannonAccuracy => CurrentWaveCannonHit / (float)CurrentWaveCannonFired;
        public bool HasBonus => CurrentWaveCannonAccuracy >= 0.75 && CurrentWaveTurretAccuracy >= 0.75f;
        public bool IsSpecialWave => SpecialWave >= 0;
        public bool[] SpecialOccurInWave = new bool[999];
        public bool HasPowerUp => PowerUp is { };


        public TurretModel CurrentTurretModel => Data.Turrets[CurrentTurret];
        public CannonModel CurrentCannonModel => Data.Cannons[CurrentCannon];
        public WaveModel CurrentWave => Data.Waves[Wave];
        public float WaveFactor => Wave / (float)Data.Waves.Length;
        public List<int> SpecialsCount = new() { 1, 1, 1, 1 };
        private List<int> _specialsBought = new() { 0, 0, 0, 0 };
        private List<int> _specialsBaseCosts = new() { 1000, 2000, 2500, 3000 };
        public List<string> SpecialsName = new() { "Air Assault", "Shield", "EMP", "Health" };
        public int SpecialDamage1 => (int)(750 * (1f + 0.1f * Wave));

        public int SpecialCost(int index) =>
            (int)(_specialsBaseCosts[index] * Mathf.Pow(1.06f, _specialsBought[index]));


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