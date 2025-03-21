using UnityEngine;

namespace FlavorfulStory
{
    public static class WorldCoordinates
    {
        private static readonly int IgnoreLayer = LayerMask.NameToLayer("Ignore Raycast");

        private static Camera _camera;

        public static void Initialize()
        {
            _camera ??= Camera.main;
        }
        public static bool GetWorldCoordinatesFromScreenPoint(Vector3 screenPointPosition, LayerMask layers, out Vector3 result)
        {
            var ray = _camera.ScreenPointToRay(screenPointPosition);
            //layers & ~(1 << IgnoreLayer)) => Все слои(layers), кроме IgnoreLayer
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layers & ~(1 << IgnoreLayer)))
            {
                result = hit.point;
                return true;
            }

            result = Vector3.zero;
            return false;
        }
    }
}