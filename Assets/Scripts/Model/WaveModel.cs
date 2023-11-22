using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Managers;

namespace Model
{
    public class WaveModel
    {
        private static GameManager Game => GameManager.Instance;
        private static DataManager Data => DataManager.Instance;

        private readonly int[] _shipsCount;
        private readonly List<int> _spawnSequence = new();
        public int Spawned, Destroyed;
        public readonly float SpawnSpeedMultiply, FogStrength;
        public int startFog = -1, endFog = -1;

        public WaveModel(int[] shipsCount, float spawnSpeedMultiply = 1, float fogStrength = 4)
        {
            _shipsCount = shipsCount;
            FogStrength = fogStrength;
            SpawnSpeedMultiply = spawnSpeedMultiply *
                                 (Game.Difficulty == 0 ? 0.95f : 1) *
                                 (Game.Difficulty == 2 ? 1.1f : 1);
            _spawnSequence.AddRange(shipsCount.SelectMany((count, index) => Enumerable.Repeat(index, count)));
            for (var i = 0; i < new Random().Next(0, 3 * Game.Difficulty); i++)
                _spawnSequence.Add(_spawnSequence.RandomItem());
            _spawnSequence.Shuffle();

            // Strong fog
            if (new Random().Next(0, 100) < 25)
            {
                startFog = new Random().Next(0, _spawnSequence.Count - 4);
                endFog = new Random().Next(startFog + 4, _spawnSequence.Count);
            }
        }

        public bool HasMore() => Spawned < ShipsCount;
        public int ShipsCount => _shipsCount.Sum();

        public KeyValuePair<int, ShipModel>? Spawn() =>
            HasMore() ? KeyValuePair.Create(Spawned, Data.Ships[_spawnSequence[Spawned++]]) : null;
    }
}