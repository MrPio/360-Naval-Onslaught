#nullable enable
using System.IO;
using UnityEngine;

namespace Managers
{
    public class IOManager
    {
        private static string _path = "";

        public static void Save(object o, string filename) =>
            File.WriteAllText(Path.Combine(Application.persistentDataPath, filename), JsonUtility.ToJson(o));

        public static T? Load<T>(string filename) where T : class =>
            File.Exists(_path = Path.Combine(Application.persistentDataPath, filename))
                ? JsonUtility.FromJson<T>(File.ReadAllText(_path))
                : null;

        public static void Delete(string filename) =>
            File.Delete(Path.Combine(Application.persistentDataPath, filename));

        public static bool Exist(string filename) =>
            File.Exists(Path.Combine(Application.persistentDataPath, filename));
    }
}