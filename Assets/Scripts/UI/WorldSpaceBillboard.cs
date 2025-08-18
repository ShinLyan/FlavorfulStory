using FlavorfulStory.Utils;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Компонент, который поворачивает объект  в сторону камеры. </summary>
    /// <remarks> Используется для создания "billboard"-эффекта. </remarks>
    public class WorldSpaceBillboard : MonoBehaviour
    {
        /// <summary> Обновляет поворот объекта каждый кадр в конце цикла, чтобы он смотрел в сторону камеры. </summary>
        private void LateUpdate() => transform.LookAt(transform.position + CameraUtils.MainCamera.transform.forward);
    }
}