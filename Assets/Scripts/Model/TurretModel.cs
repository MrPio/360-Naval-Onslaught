using System.Collections.Generic;
using System.Linq;
using Managers;
using Unity.VisualScripting;

namespace Model
{
    public class TurretModel
    {
        public static float UpgradeStep => 1.08f + (0.004f * Game.Wave); // +8%~16% for each upgrade
        private static GameManager Game => GameManager.Instance;

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

        private float _rate;
        private float _damage;
        private float _ammo;
        private float _speed;
        private float _reload;

        public int Speed => (int)(_speed *
                                  (Game.PowerUp == Bubble.PowerUp.Speed ? 2f : 1f));

        public int Rate => (int)(_rate * (Game.IsSpecialWave ? 3.5f : 1f) *
                                 (Game.PowerUp == Bubble.PowerUp.Rate ? 2f : 1f) *
                                 (Game.HasOverride ? 1.5f : 1f));

        public int Damage => (int)(_damage * (Game.IsSpecialWave ? 1.5f : 1f) *
                                   (Game.PowerUp == Bubble.PowerUp.Damage ? 2f : 1f) *
                                   (Game.HasOverride ? 0.8f : 1f));

        public int Ammo => Game.IsSpecialWave ? 9999 : (int)_ammo;
        public int Reload => (int)_reload;

        public int SpeedLevel = 1;
        public int RateLevel = 1;
        public int DamageLevel = 1;
        public int AmmoLevel = 1;
        public int ReloadLevel = 1;

        private readonly int _speedBaseCost;
        private readonly int _rateBaseCost;
        private readonly int _damageBaseCost;
        private readonly int _ammoBaseCost;
        private readonly int _reloadBaseCost;

        public int SpeedCost => (int)(_speedBaseCost * (1f + 0.425f * SpeedLevel));
        public int RateCost => (int)(_rateBaseCost * (1f + 0.425f * RateLevel));
        public int DamageCost => (int)(_damageBaseCost * (1f + 0.55f * DamageLevel));
        public int AmmoCost => (int)(_ammoBaseCost * (1f + 0.425f * AmmoLevel));
        public int ReloadCost => (int)(_reloadBaseCost * (1f + 0.425f * ReloadLevel));

        public TurretModel(string name, string sprite, string fireClip, int baseSpeed, int baseRate, int baseDamage,
            int baseAmmo, int baseReload, int speedBaseCost,
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
            _speed = _baseSpeed;
            _rate = _baseRate;
            _damage = _baseDamage;
            _ammo = _baseAmmo;
            _reload = _baseReload;
            _speedBaseCost = speedBaseCost;
            _rateBaseCost = rateBaseCost;
            _damageBaseCost = damageBaseCost;
            _ammoBaseCost = ammoBaseCost;
            _reloadBaseCost = reloadBaseCost;
            BulletSprite = bulletSprite;
            WaveUnlock = waveUnlock;
        }

        public static void UpgradeWeapon(ref float what, ref int level, int cost)
        {
            if (Game.Money < cost) return;
            Game.Money -= cost;
            if ((int)what == (int)(what * UpgradeStep))
                what = (int)what + 1;
            else
                what *= UpgradeStep;
            ++level;
        }

        public void BuySpeed() => UpgradeWeapon(ref _speed, ref SpeedLevel, SpeedCost);
        public void BuyRate() => UpgradeWeapon(ref _rate, ref RateLevel, RateCost);
        public void BuyDamage() => UpgradeWeapon(ref _damage, ref DamageLevel, DamageCost);
        public void BuyAmmo() => UpgradeWeapon(ref _ammo, ref AmmoLevel, AmmoCost);
        public void BuyReload() => UpgradeWeapon(ref _reload, ref ReloadLevel, ReloadCost);

        public bool IsLocked => Game.Wave < WaveUnlock;
    }
}