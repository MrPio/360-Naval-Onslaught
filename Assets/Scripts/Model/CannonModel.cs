using System.Collections.Generic;
using System.Linq;
using Managers;
using Unity.VisualScripting;

namespace Model
{
    public class CannonModel
    {
        private static GameManager GameManager => GameManager.Instance;

        public string Name;
        public string Sprite;
        public string FireClip;

        public int BaseSpeed;
        public int BaseDamage;
        public int BaseReload;

        public int Speed;
        public int Damage;
        public int Reload;

        public int SpeedLevel;
        public int DamageLevel;
        public int ReloadLevel;

        public Dictionary<int, int> SpeedLevelSteps;
        public Dictionary<int, int> DamageLevelSteps;
        public Dictionary<int, int> ReloadLevelSteps;

        private readonly int _speedBaseCost;
        private readonly int _damageBaseCost;
        private readonly int _reloadBaseCost;

        public int SpeedCost => (int)(_speedBaseCost * (1f + 0.35f * SpeedLevel));
        public int DamageCost => (int)(_damageBaseCost * (1f + 0.35f * DamageLevel));
        public int ReloadCost => (int)(_reloadBaseCost * (1f + 0.35f * ReloadLevel));

        public CannonModel(string name, string sprite, string fireClip, int baseSpeed, int baseDamage,
            int baseReload,
            Dictionary<int, int> speedLevelSteps,
            Dictionary<int, int> damageLevelSteps,
            Dictionary<int, int> reloadLevelSteps, int speedBaseCost, int damageBaseCost, int reloadBaseCost)
        {
            Name = name;
            Sprite = sprite;
            FireClip = fireClip;
            BaseSpeed = baseSpeed;
            BaseDamage = baseDamage;
            BaseReload = baseReload;
            Speed = BaseSpeed;
            Damage = BaseDamage;
            Reload = BaseReload;
            SpeedLevelSteps = speedLevelSteps;
            DamageLevelSteps = damageLevelSteps;
            ReloadLevelSteps = reloadLevelSteps;
            _speedBaseCost = speedBaseCost;
            _damageBaseCost = damageBaseCost;
            _reloadBaseCost = reloadBaseCost;
        }

        public void BuySpeed()
        {
            if (GameManager.Money >= SpeedCost)
            {
                GameManager.Money -= SpeedCost;
                Speed += SpeedLevelSteps.Where(entry => entry.Key >= SpeedLevel).ToList()[0].Value;
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
        
        public void BuyReload()
        {
            if (GameManager.Money >= ReloadCost)
            {
                GameManager.Money -= ReloadCost;
                Reload += ReloadLevelSteps.Where(entry => entry.Key >= ReloadLevel).ToList()[0].Value;
            }
        }
    }
}