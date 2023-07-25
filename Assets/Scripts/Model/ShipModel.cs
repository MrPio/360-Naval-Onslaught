using Managers;
using UnityEngine;

namespace Model
{
    public class ShipModel
    {
        public string Name;
        public readonly string Sprite;
        public readonly string FireClip;
        public readonly string ExplodeClip;
        public readonly string MissileSprite;
        public readonly bool HasPath;

        public readonly int BaseSpeed;
        public readonly int BaseRate;
        public readonly int BaseDamage;
        public readonly int BaseHealth;
        public readonly int BaseMoney;

        public float Delay => Mathf.Max(2f,5f - 0.125f * GameManager.Instance.Wave);
        public int Speed => (int)(BaseSpeed * (1f + 0.1f * GameManager.Instance.Wave));
        public int Rate => (int)(BaseRate * (1f + 0.1f * GameManager.Instance.Wave));
        public int Damage => (int)(BaseDamage * (1f + 0.1f * GameManager.Instance.Wave));
        public int Health => (int)(BaseHealth * (1f + 0.1f * GameManager.Instance.Wave));
        public int Money => (int)(BaseHealth * (1f + 0.05f * GameManager.Instance.Wave));

        public ShipModel(string name, string sprite, string fireClip, string explodeClip, int baseSpeed, int baseRate,
            int baseDamage, int baseHealth, int baseMoney, bool hasPath, string missileSprite)
        {
            Name = name;
            Sprite = sprite;
            FireClip = fireClip;
            ExplodeClip = explodeClip;
            BaseSpeed = baseSpeed;
            BaseRate = baseRate;
            BaseDamage = baseDamage;
            BaseHealth = baseHealth;
            BaseMoney = baseMoney;
            HasPath = hasPath;
            MissileSprite = missileSprite;
        }
    }
}