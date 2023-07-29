using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace Model
{
    public class WaveModel
    {
        private static GameManager Game => GameManager.Instance;
        private static DataManager Data => DataManager.Instance;

        private readonly List<float> _shipsChances;
        public readonly int ShipsCount;
        public int Spawned,Destroyed;
        public float SpawnSpeedMultiply;

        public WaveModel(List<float> shipsChances,int shipsCount, float spawnSpeedMultiply=1)
        {
            _shipsChances = shipsChances;
            ShipsCount = shipsCount;
            SpawnSpeedMultiply = spawnSpeedMultiply;
        }

        public bool HasMore() => Spawned < ShipsCount;

        public ShipModel Spawn()
        {
            if (!HasMore()) return null;
            
            // Choose ship
            var choice = Random.Range(0f, 1f);
            return Data.Ships.Where((_, i) => choice <= _shipsChances[i]).FirstOrDefault();
        }
    }
}