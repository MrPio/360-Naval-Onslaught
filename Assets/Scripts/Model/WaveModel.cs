using System.Collections.Generic;
using Managers;

namespace Model
{
    public class WaveModel
    {
        public List<int> Ships;
        private int _spawned;

        public WaveModel(List<int> ships)
        {
            Ships = ships;
        }

        public bool HasMore() => _spawned < Ships.Count;
        public ShipModel Spawn()
        {
            if (HasMore())
            {
                ++_spawned;
                return DataManager.Instance.Ships[Ships[_spawned - 1]];
            }

            return null;
        }
    }
}