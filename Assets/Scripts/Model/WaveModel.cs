using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Managers;

namespace Model
{
    public class WaveModel
    {
        private static DataManager Data => DataManager.Instance;

        private readonly int[] _shipsCount;
        private readonly List<int> _spawnSequence = new();
        public int Spawned, Destroyed;
        public readonly float SpawnSpeedMultiply;

        public WaveModel(int[] shipsCount, float spawnSpeedMultiply = 1)
        {
            _shipsCount = shipsCount;
            SpawnSpeedMultiply = spawnSpeedMultiply;
            _spawnSequence.AddRange(shipsCount.SelectMany((count, index) => Enumerable.Repeat(index, count)));
            _spawnSequence.Shuffle();
        }

        public bool HasMore() => Spawned < ShipsCount;
        public int ShipsCount => _shipsCount.Sum();

        public KeyValuePair<int, ShipModel>? Spawn() =>
            HasMore() ? KeyValuePair.Create(Spawned, Data.Ships[_spawnSequence[Spawned++]]) : null;
    }
}