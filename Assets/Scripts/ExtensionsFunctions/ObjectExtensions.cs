using System;

namespace ExtensionsFunctions
{
    public static class ObjectExtensions
    {
        public static T Apply<T>(this T obj, Action<T> configure)
        {
            configure?.Invoke(obj);
            return obj;
        }
    }
}