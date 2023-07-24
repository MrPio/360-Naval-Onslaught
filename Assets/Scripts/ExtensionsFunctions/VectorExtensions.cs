using UnityEngine;

namespace ExtensionsFunctions
{
    public static class VectorExtensions
    {
        public static Quaternion toQuaternion(this Vector2 vector) =>
            Quaternion.Euler(0f, 0f, Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg);

        public static Quaternion toQuaternion(this Vector3 vector) =>
            ((Vector2)vector).toQuaternion();
    }
}