using UnityEngine;

namespace FlavorfulStory.Utils
{
    /// <summary> Утилита для работы с raycast'ами от экрана к миру. </summary>
    public static class RaycastUtils
    {
        /// <summary> Слой, который исключается из raycast'ов (Ignore Raycast). </summary>
        private static readonly int IgnoreLayer = LayerMask.NameToLayer("Ignore Raycast");

        /// <summary> Попытаться получить точку пересечения луча из экрана с миром. </summary>
        /// <param name="screenPoint"> Координаты точки на экране. </param>
        /// <param name="layers"> Слой маски, по которым проводится raycast. </param>
        /// <param name="hitPoint"> Точка пересечения с миром, если успешно. </param>
        /// <returns> <c>true</c>, если пересечение найдено; иначе <c>false</c>. </returns>
        public static bool TryGetScreenPointToWorld(Vector3 screenPoint, LayerMask layers, out Vector3 hitPoint)
        {
            var ray = CameraUtils.GetScreenPointRay(screenPoint);
            hitPoint = Vector3.zero;

            if (!Physics.Raycast(ray, out var hit, 100f, layers & ~(1 << IgnoreLayer))) return false;

            hitPoint = hit.point;
            return true;
        }
    }
}