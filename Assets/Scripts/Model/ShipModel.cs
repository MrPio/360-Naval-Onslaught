using System;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Model
{
    public class ShipModel
    {
        private static GameManager Game => GameManager.Instance;
        public readonly string Name;
        public readonly string Sprite;
        public readonly string[] FireClip;
        public readonly string ExplodeClip;
        public readonly string MissileSprite;
        public readonly bool HasPath;
        public readonly Action<GameObject> StartCallback,EndPathCallback;
        public readonly int ExplosionsCount;
        public readonly float DelayMultiplier;

        private readonly int _baseSpeed;
        private readonly int _baseRate;
        private readonly int _baseDamage;
        private readonly int _baseHealth;
        private readonly int _baseMoney;

        public float Delay => 6f+Mathf.Max(0f,1f -  Game.WaveFactor)*6f;
        public int Speed => (int)(_baseSpeed * (1f + 0.05f * Game.Wave));
        public int Rate => (int)(_baseRate * (1f + 0.075f * Game.Wave));
        public int Damage => (int)(_baseDamage * (1f + 0.1f * Game.Wave));
        public int Health => (int)(_baseHealth * (1f + 0.1f * Game.Wave));
        public int Money => (int)(_baseMoney * (1f + 0.05f * Game.Wave));

        public ShipModel(string name, string sprite, string[] fireClip, string explodeClip, int baseSpeed, int baseRate,
            int baseDamage, int baseHealth, int baseMoney, bool hasPath, string missileSprite, float delayMultiplier=1f, int explosionsCount=1, Action<GameObject> startCallback=null, Action<GameObject> endPathCallback=null)
        {
            Name = name;
            Sprite = sprite;
            FireClip = fireClip;
            ExplodeClip = explodeClip;
            _baseSpeed = baseSpeed;
            _baseRate = baseRate;
            _baseDamage = baseDamage;
            _baseHealth = baseHealth;
            _baseMoney = baseMoney;
            HasPath = hasPath;
            MissileSprite = missileSprite;
            DelayMultiplier = delayMultiplier;
            ExplosionsCount = explosionsCount;
            StartCallback = startCallback;
            EndPathCallback = endPathCallback;
        }
    }
}