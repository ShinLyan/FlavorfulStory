#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.EditorTools.Attributes
{
    /// <summary> Кастомный Drawer для атрибута SteppedRange, отображающий слайдер с шагом. </summary>
    [CustomPropertyDrawer(typeof(SteppedRangeAttribute))]
    public class SteppedRangeDrawer : PropertyDrawer
    {
        /// <summary> Отрисовывает слайдер в инспекторе с учётом шага и диапазона. </summary>
        /// <param name="position"> Позиция отрисовки. </param>
        /// <param name="property"> Сериализованное поле. </param>
        /// <param name="label"> Отображаемый лейбл. </param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var steppedRangeAttribute = (SteppedRangeAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            float value = EditorGUI.Slider(position, label, property.floatValue,
                steppedRangeAttribute.Min, steppedRangeAttribute.Max);

            value = Mathf.Round(value / steppedRangeAttribute.Step) * steppedRangeAttribute.Step;
            property.floatValue = value;

            EditorGUI.EndProperty();
        }
    }
}

#endif