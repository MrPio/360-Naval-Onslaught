using System.Collections.Generic;
using System.Linq;
using Managers;
using Unity.VisualScripting;

namespace Model
{
    public class TurretModel
    {
        private static GameManager GameManager => GameManager.Instance;

        public readonly string Name;
        public readonly string Sprite;
        public readonly string BulletSprite;
        public readonly string FireClip;
        public readonly int WaveUnlock;

        private int _baseSpeed;
        private int _baseRate;
        private int _baseDamage;
        private int _baseAmmo;
        private int _baseReload;

        public int Speed;
        public int Rate;
        public int Damage;
        public int Ammo;
        public int Reload;

        public int SpeedLevel = 1;
        public int RateLevel = 1;
        public int DamageLevel = 1;
        public int AmmoLevel = 1;
        public int ReloadLevel = 1;

        public readonly Dictionary<int, int> SpeedLevelSteps;
        public readonly Dictionary<int, int> RateLevelSteps;
        public readonly Dictionary<int, int> DamageLevelSteps;
        public readonly Dictionary<int, int> AmmoLevelSteps;
        public readonly Dictionary<int, int> ReloadLevelSteps;

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
            int rateBaseCost, int damageBaseCost, int ammoBaseCost, int reloadBaseCost, string bulletSprite,
            int waveUnlock = 0)
        {
            Name = name;
            Sprite = sprite;
            FireClip = fireClip;
            _baseSpeed = baseSpeed;
            _baseRate = baseRate;
            _baseDamage = baseDamage;
            _baseAmmo = baseAmmo;
            _baseReload = baseReload;
            Speed = _baseSpeed;
            Rate = _baseRate;
            Damage = _baseDamage;
            Ammo = _baseAmmo;
            Reload = _baseReload;
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
            BulletSprite = bulletSprite;
            WaveUnlock = waveUnlock;
        }

        public void BuySpeed()
        {
            if (GameManager.Money >= SpeedCost)
            {
                GameManager.Money -= SpeedCost;
                Speed += SpeedLevelSteps.Where(entry => entry.Key >= SpeedLevel).ElementAt(0).Value;
                ++SpeedLevel;
            }
        }

        public void BuyRate()
        {
            if (GameManager.Money >= RateCost)
            {
                GameManager.Money -= RateCost;
                Rate += RateLevelSteps.Where(entry => entry.Key >= RateLevel).ElementAt(0).Value;
                ++RateLevel;
            }
        }

        public void BuyDamage()
        {
            if (GameManager.Money >= DamageCost)
            {
                GameManager.Money -= DamageCost;
                Damage += DamageLevelSteps.Where(entry => entry.Key >= DamageLevel).ElementAt(0).Value;
                ++DamageLevel;
            }
        }

        public void BuyAmmo()
        {
            if (GameManager.Money >= AmmoCost)
            {
                GameManager.Money -= AmmoCost;
                Ammo += AmmoLevelSteps.Where(entry => entry.Key >= AmmoLevel).ElementAt(0).Value;
                ++AmmoLevel;
            }
        }

        public void BuyReload()
        {
            if (GameManager.Money >= ReloadCost)
            {
                GameManager.Money -= ReloadCost;
                Reload += ReloadLevelSteps.Where(entry => entry.Key >= ReloadLevel).ElementAt(0).Value;
                ++ReloadLevel;
            }
        }

        public bool IsLocked => GameManager.Wave < WaveUnlock;
    }
}