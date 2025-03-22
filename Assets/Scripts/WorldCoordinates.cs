using UnityEngine;

namespace FlavorfulStory
{
    /// <summary>
    /// 
    /// </summary>
    public static class WorldCoordinates
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly int IgnoreLayer = LayerMask.NameToLayer("Ignore Raycast");

        /// <summary>
        /// 
        /// </summary>
        private static Camera _camera;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenPointPosition"></param>
        /// <param name="layers"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool GetWorldCoordinatesFromScreenPoint(
            Vector3 screenPointPosition, LayerMask layers, out Vector3 result)
        {
            if (!_camera) _camera = Camera.main;

            result = Vector3.zero;
            if (!Physics.Raycast(
                    _camera.ScreenPointToRay(screenPointPosition),
                    out var hit,
                    Mathf.Infinity,
                    layers & ~(1 << IgnoreLayer))) // Все слои(layers), кроме IgnoreLayer
                return false;

            result = hit.point;
            return true;
        }
    }
}