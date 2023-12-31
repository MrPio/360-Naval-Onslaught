﻿using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Model
{
    [Serializable]
    public class ShipModel
    {
        private static GameManager Game => GameManager.Instance;

        public static float SpecialSpeedMultiplier => InputManager.IsMobile ? 3.65f : 3.75f;

        public static List<float> SpecialWaveSpeedMultiplier = new()
        {
            3.35f, 4.55f, 3.25f, 3.85f, 3.85f, 3.2f,
        };

        public static float ArmoredSpeedMultiplier => 0.65f;
        public readonly string Name;
        public readonly string Sprite;
        public readonly string FoamAnim;
        public readonly string[] FireClip;
        public readonly string ExplodeClip;
        public readonly string MissileSprite;
        public readonly bool HasPath;
        public readonly Action<GameObject> StartCallback, EndPathCallback;
        public readonly int ExplosionsCount;
        public readonly float DelayMultiplier;

        private readonly int _baseSpeed;
        private readonly int _baseRate;
        private readonly int _baseDamage;
        private readonly int _baseHealth;
        private readonly int _baseMoney;

        public float Delay => 7f + Mathf.Max(0f, 1f - Game.WaveFactor) * 4f;

        public int Speed => (int)(_baseSpeed * (1f + 0.05f * Game.Wave) *
                                  (Game.IsSpecialWave
                                      ? SpecialWaveSpeedMultiplier[Game.SpecialWave] *
                                        (InputManager.IsMobile ? 0.815f : 1f)
                                      : 1f) *
                                  (Game.SpecialWave == 2 ? 1.5f : 1f) *
                                  (Game.Difficulty == 0 ? 0.85f : 1) *
                                  (Game.Difficulty == 2 ? 1.25f : 1)
            );

        public int Rate => (int)(_baseRate * (1f + 0.025f * Game.Wave) *
                                 (Game.Difficulty == 0 ? 0.9f : 1) *
                                 (Game.Difficulty == 2 ? 1.25f : 1));

        public int Damage => (int)(_baseDamage * (1f + 0.025f * Game.Wave) *
                                   (Game.IsSpecialWave ? 0.75f : 1f) *
                                   (Game.Difficulty == 0 ? 0.9f : 1) *
                                   (Game.Difficulty == 2 ? 1.25f : 1));

        public int Health => (int)(_baseHealth * (1f + (4f * Game.WaveFactor) + (0.3f * (int)(Game.Wave / 5))) *
                                   (Game.Difficulty == 0 ? 0.85f : 1) *
                                   (Game.Difficulty == 2 ? 1.25f : 1));

        public int Money => (int)(_baseMoney * (1f + 0.025f * Game.Wave) * (Game.IsSpecialWave ? 0.2f : 1f) *
                                  Game.PowerUpFactor(PowerUpModel.PowerUp.Money) *
                                  (Game.Difficulty == 0 ? 0.9f : 1) *
                                  (Game.Difficulty == 2 ? 1.25f : 1));

        public ShipModel(string name, string sprite, string foamAnim, string[] fireClip, string explodeClip,
            int baseSpeed, int baseRate,
            int baseDamage, int baseHealth, int baseMoney, bool hasPath, string missileSprite,
            float delayMultiplier = 1f, int explosionsCount = 1, Action<GameObject> startCallback = null,
            Action<GameObject> endPathCallback = null)
        {
            Name = name;
            Sprite = sprite;
            FoamAnim = foamAnim;
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