using Model;

namespace Managers
{
    public class GameManager
    {
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
        public int Money = 0;
        public int CurrentTurret = 1;
        public int CurrentCannon = 2;
        public int HealthStep => (int)(_healthBaseStep * (1f + 0.35f * HealthLevel));
        public int HealthCost => (int)(_healthBaseCost * (1f + 0.25f * HealthLevel));
        public int RepairCost => (int)((MaxHealth - Health) * (1f + 0.35f * _repairLevel));

        public TurretModel CurrentTurretModel => DataManager.Instance.Turrets[CurrentTurret];
        public CannonModel CurrentCannonModel => DataManager.Instance.Cannons[CurrentCannon];
        public WaveModel CurrentWave => DataManager.Instance.Waves[Wave];

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
            if (Money >= RepairCost && Health<MaxHealth)
            {
                Money -= RepairCost;
                Health = MaxHealth;
                _repairLevel++;
            }
        }
    }
}