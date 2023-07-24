using System.Collections.Generic;
using UnityEngine;

namespace ExtensionsFunctions
{
    public static class ListExtensions
    {
        public static T RandomItem<T>(this List<T> list) =>
            list[Random.Range(0, list.Count)];
    }
}