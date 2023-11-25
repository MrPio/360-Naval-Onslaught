using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace ExtensionsFunctions
{
    public static class ListExtensions
    {
        public static T RandomItem<T>(this List<T> list) =>
            list[Random.Range(0, list.Count)];

        public static void Shuffle<T>(this List<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                var randomIndex = Random.Range(0, --n + 1);
                (list[randomIndex], list[n]) = (list[n], list[randomIndex]);
            }
        }
        
        public static void ForEach<T>(this List<T> list,Action<T,int> action)
        {
            for (var i = 0; i < list.Count; i++)
                action(list[i], i);
        }
    }
}