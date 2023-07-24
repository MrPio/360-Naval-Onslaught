using System.Collections.Generic;
using System.Linq;
using Managers;
using Unity.VisualScripting;

namespace Model
{
    public class TurretModel
    {
        private static GameManager GameManager => GameManager.Instance;

        public string Name;
        public string Sprite;
        public string Clip;
        
        public int BaseSpeed;
        public int BaseRate;
        public int BaseDamage;
        public int BaseAmmo;

        public int Speed;
        public int Rate;
        public int Damage;
        public int Ammo;

        public int SpeedLevel;
        public int RateLevel;
        public int DamageLevel;
        public int AmmoLevel;

        public Dictionary<int, int> SpeedLevelSteps;
        public Dictionary<int, int> RateLevelSteps;
        public Dictionary<int, int> DamageLevelSteps;
        public Dictionary<int, int> AmmoLevelSteps;

        private int speedBaseCost;
        private int rateBaseCost;
        private int damageBaseCost;
        private int ammoBaseCost;

        public int SpeedCost => (int)(speedBaseCost * (1f + 0.35f * SpeedLevel));
        public int RateCost => (int)(rateBaseCost * (1f + 0.35f * RateLevel));
        public int DamageCost => (int)(damageBaseCost * (1f + 0.35f * DamageLevel));
        public int AmmoCost => (int)(ammoBaseCost * (1f + 0.35f * AmmoLevel));

        public TurretModel(string name, string sprite,string clip, int baseSpeed, int baseRate, int baseDamage, int baseAmmo,
            Dictionary<int, int> speedLevelSteps, Dictionary<int, int> rateLevelSteps,
            Dictionary<int, int> damageLevelSteps, Dictionary<int, int> ammoLevelSteps, int speedBaseCost,
            int rateBaseCost, int damageBaseCost, int ammoBaseCost)
        {
            Name = name;
            Sprite = sprite;
            Clip = clip;
            BaseSpeed = baseSpeed;
            BaseRate = baseRate;
            BaseDamage = baseDamage;
            BaseAmmo = baseAmmo;
            Speed = BaseSpeed;
            Rate = BaseRate;
            Damage = BaseDamage;
            Ammo = BaseAmmo;
            SpeedLevelSteps = speedLevelSteps;
            RateLevelSteps = rateLevelSteps;
            DamageLevelSteps = damageLevelSteps;
            AmmoLevelSteps = ammoLevelSteps;
            this.speedBaseCost = speedBaseCost;
            this.rateBaseCost = rateBaseCost;
            this.damageBaseCost = damageBaseCost;
            this.ammoBaseCost = ammoBaseCost;
        }

        public void BuySpeed()
        {
            if (GameManager.Money >= SpeedCost)
            {
                GameManager.Money -= SpeedCost;
                Speed += SpeedLevelSteps.Where(entry => entry.Key >= SpeedLevel).ToList()[0].Value;
            }
        }

        public void BuyRate()
        {
            if (GameManager.Money >= RateCost)
            {
                GameManager.Money -= RateCost;
                Rate += RateLevelSteps.Where(entry => entry.Key >= RateLevel).ToList()[0].Value;
            }
        }

        public void BuyDamage()
        {
            if (GameManager.Money >= DamageCost)
            {
                GameManager.Money -= DamageCost;
                Damage += DamageLevelSteps.Where(entry => entry.Key >= DamageLevel).ToList()[0].Value;
            }
        }

        public void BuyAmmo()
        {
            if (GameManager.Money >= AmmoCost)
            {
                GameManager.Money -= AmmoCost;
                Ammo += AmmoLevelSteps.Where(entry => entry.Key >= AmmoLevel).ToList()[0].Value;
            }
        }
    }
}