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
        public string FireClip,CannonBallSprite;
        public int WaveUnlock;

        public int BaseSpeed;
        public int BaseDamage;
        public int BaseReload;
        public int BaseRadius;

        public int Speed;
        public int Damage;
        public int Reload;
        public int Radius;

        public int SpeedLevel=1;
        public int DamageLevel=1;
        public int ReloadLevel=1;
        public int RadiusLevel=1;

        public Dictionary<int, int> SpeedLevelSteps;
        public Dictionary<int, int> DamageLevelSteps;
        public Dictionary<int, int> ReloadLevelSteps;
        public Dictionary<int, int> RadiusLevelSteps;

        private readonly int _speedBaseCost;
        private readonly int _damageBaseCost;
        private readonly int _reloadBaseCost;
        private readonly int _radiusBaseCost;

        public int SpeedCost => (int)(_speedBaseCost * (1f + 0.35f * SpeedLevel));
        public int DamageCost => (int)(_damageBaseCost * (1f + 0.35f * DamageLevel));
        public int ReloadCost => (int)(_reloadBaseCost * (1f + 0.35f * ReloadLevel));
        public int RadiusCost => (int)(_radiusBaseCost * (1f + 0.35f * RadiusLevel));

        public CannonModel(string name, string sprite, string fireClip, int baseSpeed, int baseDamage,
            int baseReload, int baseRadius, Dictionary<int, int> speedLevelSteps, Dictionary<int, int> damageLevelSteps,
            Dictionary<int, int> reloadLevelSteps, Dictionary<int, int> radiusLevelSteps, int speedBaseCost,
            int damageBaseCost, int reloadBaseCost, int radiusBaseCost, string cannonBallSprite, int waveUnlock=0)
        {
            Name = name;
            Sprite = sprite;
            FireClip = fireClip;
            BaseSpeed = baseSpeed;
            BaseDamage = baseDamage;
            BaseReload = baseReload;
            BaseRadius = baseRadius;
            Speed = BaseSpeed;
            Damage = BaseDamage;
            Reload = BaseReload;
            Radius = BaseRadius;
            SpeedLevelSteps = speedLevelSteps;
            DamageLevelSteps = damageLevelSteps;
            ReloadLevelSteps = reloadLevelSteps;
            RadiusLevelSteps = radiusLevelSteps;
            _speedBaseCost = speedBaseCost;
            _damageBaseCost = damageBaseCost;
            _reloadBaseCost = reloadBaseCost;
            _radiusBaseCost = radiusBaseCost;
            CannonBallSprite = cannonBallSprite;
            WaveUnlock = waveUnlock;
        }

        public void BuySpeed()
        {
            if (GameManager.Money >= SpeedCost)
            {
                GameManager.Money -= SpeedCost;
                Speed += SpeedLevelSteps.Where(entry => entry.Key >= SpeedLevel).ElementAt(0).Value;
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

        public void BuyReload()
        {
            if (GameManager.Money >= ReloadCost)
            {
                GameManager.Money -= ReloadCost;
                Reload += ReloadLevelSteps.Where(entry => entry.Key >= ReloadLevel).ElementAt(0).Value;
            }
        }        
        public void BuyRadius()
        {
            if (GameManager.Money >= RadiusCost)
            {
                GameManager.Money -= RadiusCost;
                Radius += RadiusLevelSteps.Where(entry => entry.Key >= RadiusLevel).ElementAt(0).Value;
            }
        }
        public bool IsLocked => GameManager.Wave < WaveUnlock;
    }
}