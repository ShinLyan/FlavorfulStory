using UnityEngine;

namespace FlavorfulStory.Utils
{
    /// <summary> Вспомогательный класс мировых координат. </summary>
    /// <remarks> Занимается кастом координат из одного типа в другой. </remarks>
    public static class WorldCoordinates
    {
        /// <summary> Слой, игнорируемый рейкастом. </summary>
        private static readonly int IgnoreLayer = LayerMask.NameToLayer("Ignore Raycast");

        /// <summary> Камера для рейкаста. </summary>
        private static Camera _camera;

        /// <summary> Получить координаты точки экрана в игровом мире. </summary>
        /// <param name="screenPointPosition"> Точка экрана. </param>
        /// <param name="layers"> слои, которые учитываются при рейкасте. </param>
        /// <param name="result"> Координаты точки в игровом мире. </param>
        /// <returns></returns>
        public static bool GetWorldCoordinatesFromScreenPoint(
            Vector3 screenPointPosition, LayerMask layers, out Vector3 result)
        {
            if (!_camera) _camera = Camera.main;

            result = Vector3.zero;
            if (!Physics.Raycast(
                    _camera.ScreenPointToRay(screenPointPosition),
                    out var hit,
                    100f,
                    layers & ~(1 << IgnoreLayer))) // Все слои(layers), кроме IgnoreLayer
                return false;

            result = hit.point;
            return true;
        }
    }
}