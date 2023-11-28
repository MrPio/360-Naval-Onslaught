using System.Linq;
using Managers;

namespace Model
{
    public class PowerUpModel
    {
        public enum PowerUp
        {
            Satellite,
            Damage,
            Rate,
            Speed,
            Health
        }

        private static GameManager Game => GameManager.Instance;
        private static DataManager Data => DataManager.Instance;

        private readonly float _duration;
        private readonly int _amount, _upgradeBaseCost;
        public float _strength = 1f;

        public readonly PowerUp Type;
        public readonly string Description;
        public readonly string Sprite;
        public readonly float Multiplier;
        public readonly int UnlockCost;
        public int Level;
        public bool IsLocked = true;

        public int Health => (int)(120 * (1f + 4f * Game.WaveFactor));

        public float StrengthStepFactor = 0.25f,AmountStepFactor=1.15f,DurationStepFactor=1.15f;
        public float Lifespan => 7f *
                                 (Game.Difficulty == 0 ? 1.15f : 1) *
                                 (Game.Difficulty == 2 ? 0.87f : 1);

        public float Duration => _duration * (1f + Level * DurationStepFactor) *
                                 (Game.Difficulty == 0 ? 1.2f : 1) *
                                 (Game.Difficulty == 2 ? 0.83f : 1);

        public int Amount => (int)(_amount * (1f + Level * AmountStepFactor) *
                                   (Game.Difficulty == 0 ? 1.2f : 1) *
                                   (Game.Difficulty == 2 ? 0.83f : 1));

        public int Strength => (int)(_strength * (1f + Level * StrengthStepFactor));

        public int Index => Data.PowerUps.ToList().IndexOf(this);
        public string Name => Type.ToString().ToLower();
        public int UpgradeCost => (int)(_upgradeBaseCost * (1f + 0.425f * Level));
        public bool HasMultiplier => Multiplier >= 0;

        public PowerUpModel(PowerUp type, string description, string sprite, float duration, int upgradeBaseCost,
            int unlockCost, int amount = 0,
            float multiplier = -1f)
        {
            Type = type;
            Description = description;
            Sprite = sprite;
            _duration = duration;
            _upgradeBaseCost = upgradeBaseCost;
            UnlockCost = unlockCost;
            _amount = amount;
            Multiplier = multiplier;
        }

        public void Upgrade()
        {
            if (Game.Money < UpgradeCost) return;
            Game.Money -= UpgradeCost;
            ++Level;
        }
    }
}