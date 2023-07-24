using System.Linq;
using UnityEngine;

namespace ExtensionsFunctions
{
    public static class CameraExtensions
    {
        public static Quaternion AngleToMouse(this Camera camera, Vector3? from = null)
        {
            var mousePositionWorld = camera.ScreenToWorldPoint(Input.mousePosition);
            var directionToMouse = mousePositionWorld - from ?? Vector3.zero;
            var angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, angle);
        }

        public static Vector3 RandomBoundaryPoint(this Camera camera)
        {
            var width = camera.GetWidth();
            var height = camera.GetHeight();
            return Random.Range(0, 2) == 0
                ? new Vector2(Random.Range(-width, width), Random.Range(0, 2) == 1 ? height : -height)
                : new Vector2(Random.Range(0, 2) == 1 ? width : -width, Random.Range(-height, height));
        }

        public static float GetHeight(this Camera camera) => camera.orthographicSize;
        public static float GetWidth(this Camera camera) => camera.orthographicSize * camera.aspect;
    }
}