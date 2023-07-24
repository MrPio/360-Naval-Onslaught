using UnityEngine;

namespace Managers
{
    public class PlayerManager
    {
        private static PlayerManager _instance;

        private PlayerManager()
        {
            Ammo = MaxAmmo;
        }

        public static PlayerManager Instance => _instance ??= new PlayerManager();

        public float FireRate=10;
        public float BulletSpeed=8;
        public int Health=5;
        public int MaxHealth=5;
        public int MaxAmmo=50;
        public int Ammo;
        public int Damage=6;
        
    }
}