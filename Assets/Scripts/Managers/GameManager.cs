using System.Collections.Generic;
using Model;
using UnityEngine;

namespace Managers
{
    public class GameManager
    {
        private static DataManager Data => DataManager.Instance;
        public static void Reset() => _instance = new GameManager();

        private static GameManager _instance;

        private GameManager()
        {
            Ammo = CurrentTurretModel.Ammo;
            CannonAmmo = 1;
            Health = MaxHealth;
        }

        public static GameManager Instance => _instance ??= new GameManager();

        public int Health;
        public int MaxHealth = 3500;
        private int _healthBaseStep = 175;
        private int _healthBaseCost = 500;
        private int _repairLevel = 1;
        public int HealthLevel = 1;
        public int Wave = 0;
        public int Ammo, CannonAmmo;
        public int Money = 9990;
        public int CurrentTurret = 3;
        public int CurrentCannon = 2;
        public int Score;
        public int HealthStep => (int)(_healthBaseStep * (1f + 0.35f * HealthLevel));
        public int HealthCost => (int)(_healthBaseCost * (1f + 0.25f * HealthLevel));
        public int RepairCost => (int)(0.28f * (MaxHealth - Health) * (1f + 0.15f * _repairLevel));

        public TurretModel CurrentTurretModel => Data.Turrets[CurrentTurret];
        public CannonModel CurrentCannonModel => Data.Cannons[CurrentCannon];
        public WaveModel CurrentWave => Data.Waves[Wave];
        public float WaveFactor => Wave / (float)Data.Waves.Length;
        public List<int> SpecialsCount = new() { 4, 4,4, 4 };
        private List<int> _specialsBought = new() { 0, 0, 0, 0 };
        private List<int> _specialsBaseCosts = new() { 1000, 2000, 2500, 3000 };
        public List<string> SpecialsName = new() { "Air Assault", "Shield", "EMP", "Health" };
        public int SpecialDamage1 = 750;
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