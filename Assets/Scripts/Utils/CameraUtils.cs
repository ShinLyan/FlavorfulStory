using FlavorfulStory.InputSystem;
using UnityEngine;

namespace FlavorfulStory.Utils
{
    /// <summary> Утилита для работы с камерой и построением лучей. </summary>
    /// <remarks> Отвечает только за камеру и лучи. </remarks>
    public static class CameraUtils
    {
        /// <summary> Кешированная основная камера. </summary>
        private static Camera _mainCamera;

        /// <summary> Ссылка на основную камеру. </summary>
        private static Camera MainCamera
        {
            get
            {
                if (!_mainCamera) _mainCamera = Camera.main;
                return _mainCamera;
            }
        }

        /// <summary> Построить луч от позиции курсора мыши. </summary>
        /// <returns> Луч от камеры через позицию мыши. </returns>
        public static Ray GetMouseRay() => MainCamera.ScreenPointToRay(InputWrapper.GetMousePosition());

        /// <summary> Построить луч от заданной точки экрана. </summary>
        /// <param name="screenPoint"> Координаты точки на экране. </param>
        /// <returns> Луч от камеры через заданную точку. </returns>
        public static Ray GetScreenPointRay(Vector3 screenPoint) => MainCamera.ScreenPointToRay(screenPoint);
    }
}