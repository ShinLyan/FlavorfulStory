using System;
using UnityEngine;

namespace FlavorfulStory.Control.CursorSystem
{
    /// <summary> Отображение курсора в зависимости от его типа. </summary>
    [Serializable]
    public struct CursorMapping
    {
        /// <summary> Тип курсора. </summary>
        [field: SerializeField]
        public CursorType Type { get; private set; }

        /// <summary> Текстура курсора, соответствующая заданному типу. </summary>
        [field: SerializeField]
        public Texture2D Texture { get; private set; }

        /// <summary> Смещение точки курсора относительно верхнего левого угла текстуры. </summary>
        [field: SerializeField]
        public Vector2 Hotspot { get; private set; }
    }
}