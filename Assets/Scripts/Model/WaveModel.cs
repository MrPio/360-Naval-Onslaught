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

        public WaveModel(List<float> shipsChances,int shipsCount)
        {
            _shipsChances = shipsChances;
            ShipsCount = shipsCount;
        }

        public bool HasMore() => Spawned < ShipsCount;

        public ShipModel Spawn()
        {
            if (!HasMore()) return null;
            
            // Choose ship
            var choice = Random.Range(0f, 1f);
            return Data.Ships.Where((t, i) => choice <= _shipsChances[i]).FirstOrDefault();
        }
    }
}