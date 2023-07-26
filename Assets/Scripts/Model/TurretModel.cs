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
        public string FireClip;

        public int BaseSpeed;
        public int BaseRate;
        public int BaseDamage;
        public int BaseAmmo;
        public int BaseReload;

        public int Speed;
        public int Rate;
        public int Damage;
        public int Ammo;
        public int Reload;

        public int SpeedLevel=1;
        public int RateLevel=1;
        public int DamageLevel=1;
        public int AmmoLevel=1;
        public int ReloadLevel=1;

        public Dictionary<int, int> SpeedLevelSteps;
        public Dictionary<int, int> RateLevelSteps;
        public Dictionary<int, int> DamageLevelSteps;
        public Dictionary<int, int> AmmoLevelSteps;
        public Dictionary<int, int> ReloadLevelSteps;

        private readonly int _speedBaseCost;
        private readonly int _rateBaseCost;
        private readonly int _damageBaseCost;
        private readonly int _ammoBaseCost;
        private readonly int _reloadBaseCost;

        public int SpeedCost => (int)(_speedBaseCost * (1f + 0.35f * SpeedLevel));
        public int RateCost => (int)(_rateBaseCost * (1f + 0.35f * RateLevel));
        public int DamageCost => (int)(_damageBaseCost * (1f + 0.35f * DamageLevel));
        public int AmmoCost => (int)(_ammoBaseCost * (1f + 0.35f * AmmoLevel));
        public int ReloadCost => (int)(_reloadBaseCost * (1f + 0.35f * ReloadLevel));

        public TurretModel(string name, string sprite, string fireClip, int baseSpeed, int baseRate, int baseDamage,
            int baseAmmo, int baseReload,
            Dictionary<int, int> speedLevelSteps, Dictionary<int, int> rateLevelSteps,
            Dictionary<int, int> damageLevelSteps, Dictionary<int, int> ammoLevelSteps,
            Dictionary<int, int> reloadLevelSteps, int speedBaseCost,
            int rateBaseCost, int damageBaseCost, int ammoBaseCost, int reloadBaseCost)
        {
            Name = name;
            Sprite = sprite;
            FireClip = fireClip;
            BaseSpeed = baseSpeed;
            BaseRate = baseRate;
            BaseDamage = baseDamage;
            BaseAmmo = baseAmmo;
            BaseReload = baseReload;
            Speed = BaseSpeed;
            Rate = BaseRate;
            Damage = BaseDamage;
            Ammo = BaseAmmo;
            Reload = BaseReload;
            SpeedLevelSteps = speedLevelSteps;
            RateLevelSteps = rateLevelSteps;
            DamageLevelSteps = damageLevelSteps;
            AmmoLevelSteps = ammoLevelSteps;
            ReloadLevelSteps = reloadLevelSteps;
            _speedBaseCost = speedBaseCost;
            _rateBaseCost = rateBaseCost;
            _damageBaseCost = damageBaseCost;
            _ammoBaseCost = ammoBaseCost;
            _reloadBaseCost = reloadBaseCost;
        }

        public void BuySpeed()
        {
            if (GameManager.Money >= SpeedCost)
            {
                GameManager.Money -= SpeedCost;
                Speed += SpeedLevelSteps.Where(entry => entry.Key >= SpeedLevel).ElementAt(0).Value;
            }
        }

        public void BuyRate()
        {
            if (GameManager.Money >= RateCost)
            {
                GameManager.Money -= RateCost;
                Rate += RateLevelSteps.Where(entry => entry.Key >= RateLevel).ElementAt(0).Value;
            }
        }

        public void BuyDamage()
        {
            if (GameManager.Money >= DamageCost)
            {
                GameManager.Money -= DamageCost;
                Damage += DamageLevelSteps.Where(entry => entry.Key >= DamageLevel).ElementAt(0).Value;
            }
        }

        public void BuyAmmo()
        {
            if (GameManager.Money >= AmmoCost)
            {
                GameManager.Money -= AmmoCost;
                Ammo += AmmoLevelSteps.Where(entry => entry.Key >= AmmoLevel).ElementAt(0).Value;
            }
        }
        
        public void BuyReload()
        {
            if (GameManager.Money >= ReloadCost)
            {
                GameManager.Money -= ReloadCost;
                Reload += ReloadLevelSteps.Where(entry => entry.Key >= ReloadLevel).ElementAt(0).Value;
            }
        }
    }
}