using Model;

namespace Managers
{
    public class PlayerManager
    {
        private static PlayerManager _instance;

        private PlayerManager()
        {
            Ammo = CurrentTurret.Ammo;
            Health = MaxHealth;
        }

        public static PlayerManager Instance => _instance ??= new PlayerManager();

        public int Health;
        public int MaxHealth = 500;
        public int Wave = 0;
        public int Ammo;
        public int Money = 500;
        public TurretModel CurrentTurret = DataManager.Instance.Turrets[0];
        public WaveModel CurrentWave => DataManager.Instance.Waves[Wave];
    }
}