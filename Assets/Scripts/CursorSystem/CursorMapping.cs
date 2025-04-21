using System;
using UnityEngine;

namespace FlavorfulStory.CursorSystem
{
    /// <summary> Настройки отображения курсора для конкретного типа взаимодействия. </summary>
    [Serializable]
    public struct CursorMapping
    {
        /// <summary> Тип курсора. </summary>
        /// <remarks> Определяет, в какой ситуации будет использоваться этот курсор. Например: наведение на NPC,
        /// предмет, врага, землю и т.п. Типы задаются в перечислении <see cref="CursorType"/>. </remarks>
        [field: Tooltip("Тип взаимодействия, для которого отображается этот курсор " +
                        "(например, Диалог, Сбор, Атака и т.п.)."), SerializeField]
        public CursorType Type { get; private set; }

        /// <summary> Текстура курсора. </summary>
        /// <remarks> Это изображение, которое появится в игре вместо стандартного системного курсора
        /// при данном типе взаимодействия. Например: иконка руки для диалога или молотка для починки. </remarks>
        [field: Tooltip("Текстура для отображения курсора при выбранном типе."), SerializeField]
        public Texture2D Texture { get; private set; }

        /// <summary> Горячая точка (hotspot) курсора. </summary>
        /// <remarks> Указывает смещение точки "кончика" курсора относительно левого верхнего угла текстуры.
        /// Например, если поставить (16,16) для текстуры 32x32, то точка попадания будет по центру текстуры.
        /// Обычно выбирается в центре или на остриё курсора для точного взаимодействия. </remarks>
        [field: Tooltip("Смещение точки, которая будет использоваться как \"кончик\" курсора (в пикселях)."),
                SerializeField]
        public Vector2 Hotspot { get; private set; }
    }
}