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
        private readonly int _amount;

        public readonly PowerUp Type;
        public readonly string Sprite;
        public readonly float Multiplier;

        public int Health => (int)(120 * (1f + 4f * Game.WaveFactor));

        public float Lifespan => 7f *
                                 (Game.Difficulty == 0 ? 1.15f : 1) *
                                 (Game.Difficulty == 2 ? 0.87f : 1);

        public float Duration => _duration *
                                 (Game.Difficulty == 0 ? 1.2f : 1) *
                                 (Game.Difficulty == 2 ? 0.83f : 1);

        public int Amount => (int)(_amount *
                                   (Game.Difficulty == 0 ? 1.2f : 1) *
                                   (Game.Difficulty == 2 ? 0.83f : 1));

        public int Index => Data.PowerUps.ToList().IndexOf(this);
        public string Name => Type.ToString().ToLower();
        public bool HasMultiplier => Multiplier >= 0;

        public PowerUpModel(PowerUp type, string sprite, float duration, int amount = 0, float multiplier = -1f)
        {
            Type = type;
            Sprite = sprite;
            _duration = duration;
            _amount = amount;
            Multiplier = multiplier;
        }
    }
}