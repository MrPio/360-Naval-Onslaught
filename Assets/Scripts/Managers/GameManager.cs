using Model;

namespace Managers
{
    public class GameManager
    {
        private static GameManager _instance;

        private GameManager()
        {
            Ammo = CurrentTurretModel.Ammo;
            Health = MaxHealth;
        }

        public static GameManager Instance => _instance ??= new GameManager();

        public int Health;
        public int MaxHealth = 500;
        public int Wave = 0;
        public int Ammo;
        public int Money = 500;
        public int CurrentTurret = 0;
        public int CurrentCannon = 0;
        public TurretModel CurrentTurretModel => DataManager.Instance.Turrets[0];
        public CannonModel CurrentCannonModel => DataManager.Instance.Cannons[0];
        public WaveModel CurrentWave => DataManager.Instance.Waves[Wave];
    }
}