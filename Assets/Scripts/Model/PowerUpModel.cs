using System;
using System.Linq;
using Managers;

namespace Model
{
    [Serializable]
    public class PowerUpModel
    {
        private static GameManager Game => GameManager.Instance;
        private static DataManager Data => DataManager.Instance;

        public enum PowerUp
        {
            Speed,
            Rate,
            Damage,
            Reload,
            Satellite,
            Critical,
            Health,
            Money
        }


        private readonly int _upgradeBaseCost;
        public readonly float BaseDuration;
        public readonly float BaseStrength;

        public readonly PowerUp Type;
        public readonly string Description;
        public readonly string Sprite;
        public readonly bool IsMultiplier;
        public readonly int UnlockCost, UnlockDiamondCost;
        public int Level;
        public bool IsLocked;
        public int Health => (int)(120 * (1f + 4f * Game.WaveFactor));

        public float StrengthStepFactor = 0.25f, DurationStepFactor = 0.15f;

        public float Lifespan => 7f *
                                 (Game.Difficulty == 0 ? 1.15f : 1) *
                                 (Game.Difficulty == 2 ? 0.87f : 1);

        public float Duration => BaseDuration * (1f + Level * DurationStepFactor) *
                                 (Game.Difficulty == 0 ? 1.2f : 1) *
                                 (Game.Difficulty == 2 ? 0.83f : 1);

        public float Strength => BaseStrength * (1f + Level * StrengthStepFactor);

        public int Index => Data.PowerUps.ToList().IndexOf(this);
        public string Name => Type.ToString().ToLower();
        public int UpgradeCost => (int)(_upgradeBaseCost * (1f + 0.425f * Level));
        public bool HasDuration => BaseDuration > 0;

        public PowerUpModel(PowerUp type, string description, string sprite, int upgradeBaseCost,
            int unlockCost, int unlockDiamondCost = 1, float baseDuration = 20f, float baseStrength = 1.5f,
            bool isMultiplier = false,
            bool isLocked = true)
        {
            Type = type;
            Description = description;
            Sprite = sprite;
            BaseDuration = baseDuration;
            BaseStrength = baseStrength;
            _upgradeBaseCost = upgradeBaseCost;
            UnlockCost = unlockCost;
            UnlockDiamondCost = unlockDiamondCost;
            IsMultiplier = isMultiplier;
            IsLocked = isLocked;
        }

        public void Unlock()
        {
            if (Game.Money < UnlockCost || Game.Diamonds < UnlockDiamondCost || !IsLocked) return;
            Game.Money -= UnlockCost;
            Game.Diamonds -= UnlockDiamondCost;
            IsLocked = false;
        }

        public void Upgrade()
        {
            if (Game.Money < UpgradeCost) return;
            Game.Money -= UpgradeCost;
            ++Level;
        }
    }
}