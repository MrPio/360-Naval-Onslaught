using System.Collections.Generic;
using System.Linq;
using Managers;
using Unity.VisualScripting;

namespace Model
{
    public class CannonModel
    {
        private static GameManager Game => GameManager.Instance;

        public string Name;
        public string Sprite;
        public string FireClip, CannonBallSprite;
        public int WaveUnlock;

        public int BaseSpeed;
        public int BaseDamage;
        public int BaseReload;
        public int BaseRadius;

        private float _speed;
        private float _reload;
        private float _radius;
        private float _damage;
        public int Speed => (int)(_speed * (Game.IsSpecialWave ? 3f : 1f));
        public int Damage => (int)(_damage * (Game.PowerUp == Bubble.PowerUp.Damage ? 2f : 1f));

        public int Reload =>
            (int)(_reload * (Game.IsSpecialWave ? 5f : 1f) * (Game.PowerUp == Bubble.PowerUp.Rate ? 2f : 1f));

        public int Radius => (int)(_radius * (Game.IsSpecialWave ? 3.25f : 1f));

        public int SpeedLevel = 1;
        public int DamageLevel = 1;
        public int ReloadLevel = 1;
        public int RadiusLevel = 1;

        private readonly int _speedBaseCost;
        private readonly int _damageBaseCost;
        private readonly int _reloadBaseCost;
        private readonly int _radiusBaseCost;

        public int SpeedCost => (int)(_speedBaseCost * (1f + 0.35f * SpeedLevel));
        public int DamageCost => (int)(_damageBaseCost * (1f + 0.35f * DamageLevel));
        public int ReloadCost => (int)(_reloadBaseCost * (1f + 0.35f * ReloadLevel));
        public int RadiusCost => (int)(_radiusBaseCost * (1f + 0.35f * RadiusLevel));

        public CannonModel(string name, string sprite, string fireClip, int baseSpeed, int baseDamage,
            int baseReload, int baseRadius, int speedBaseCost,
            int damageBaseCost, int reloadBaseCost, int radiusBaseCost, string cannonBallSprite, int waveUnlock = 0)
        {
            Name = name;
            Sprite = sprite;
            FireClip = fireClip;
            BaseSpeed = baseSpeed;
            BaseDamage = baseDamage;
            BaseReload = baseReload;
            BaseRadius = baseRadius;
            _speed = BaseSpeed;
            _damage = BaseDamage;
            _reload = BaseReload;
            _radius = BaseRadius;
            _speedBaseCost = speedBaseCost;
            _damageBaseCost = damageBaseCost;
            _reloadBaseCost = reloadBaseCost;
            _radiusBaseCost = radiusBaseCost;
            CannonBallSprite = cannonBallSprite;
            WaveUnlock = waveUnlock;
        }

        public void BuySpeed() => TurretModel.UpgradeWeapon(ref _speed, ref SpeedLevel, SpeedCost);
        public void BuyDamage() => TurretModel.UpgradeWeapon(ref _damage, ref DamageLevel, DamageCost);
        public void BuyReload() => TurretModel.UpgradeWeapon(ref _reload, ref ReloadLevel, ReloadCost);
        public void BuyRadius() => TurretModel.UpgradeWeapon(ref _radius, ref RadiusLevel, RadiusCost);

        public bool IsLocked => Game.Wave < WaveUnlock;
    }
}