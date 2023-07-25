using System.Collections.Generic;
using Managers;

namespace Model
{
    public class WaveModel
    {
        public List<int> Ships;
        static public int Spawned;

        public WaveModel(List<int> ships)
        {
            Ships = ships;
        }

        public bool HasMore() => Spawned < Ships.Count;
        public ShipModel Spawn()
        {
            if (HasMore())
            {
                ++Spawned;
                return DataManager.Instance.Ships[Ships[Spawned - 1]];
            }

            return null;
        }
    }
}