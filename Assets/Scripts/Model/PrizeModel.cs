using System.Collections.Generic;

namespace Model
{
    public class PrizeModel
    {
        public enum PrizeType
        {
            Money,
            Score,
            Diamond
        }

        public static Dictionary<PrizeType, string> SlotSprites = new()
        {
            [PrizeType.Money] = "Sprites/hud/cash",
            [PrizeType.Score] = "Sprites/hud/points",
            [PrizeType.Diamond] = "Sprites/power_up/diamond",
        };

        public int Amount;
        public string AmountText => (Type is PrizeType.Diamond ? Amount : (Amount / 100)).ToString();
        public PrizeType Type;
        public bool IsImpossible;

        public PrizeModel(int amount, PrizeType type, bool isImpossible = false)
        {
            Amount = amount;
            Type = type;
            IsImpossible = isImpossible;
        }
    }
}