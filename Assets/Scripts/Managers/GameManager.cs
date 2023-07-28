using Model;

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
        public int MaxHealth = 10;
        private int _healthBaseStep = 175;
        private int _healthBaseCost = 500;
        private int _repairLevel = 1;
        public int HealthLevel = 1;
        public int Wave = 0;
        public int Ammo, CannonAmmo;
        public int Money = 0;
        public int CurrentTurret = 0;
        public int CurrentCannon = 2;
        public float Score;
        public int HealthStep => (int)(_healthBaseStep * (1f + 0.35f * HealthLevel));
        public int HealthCost => (int)(_healthBaseCost * (1f + 0.25f * HealthLevel));
        public int RepairCost => (int)(0.28f*(MaxHealth - Health) * (1f + 0.15f * _repairLevel));

        public TurretModel CurrentTurretModel => Data.Turrets[CurrentTurret];
        public CannonModel CurrentCannonModel => Data.Cannons[CurrentCannon];
        public WaveModel CurrentWave => Data.Waves[Wave];
        public float WaveFactor => Wave / (float)Data.Waves.Length;

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