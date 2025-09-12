#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.Utils.Attributes
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
            var stepped = (SteppedRangeAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                {
                    float raw = EditorGUI.Slider(position, label, property.floatValue, stepped.Min, stepped.Max);
                    float steppedValue = Mathf.Round(raw / stepped.Step) * stepped.Step;
                    property.floatValue = steppedValue;
                    break;
                }

                case SerializedPropertyType.Integer:
                {
                    float raw = EditorGUI.Slider(position, label, property.intValue, stepped.Min, stepped.Max);
                    int steppedValue = Mathf.RoundToInt(Mathf.Round(raw / stepped.Step) * stepped.Step);
                    property.intValue = steppedValue;
                    break;
                }

                default:
                    EditorGUI.LabelField(position, label.text, "SteppedRange supports only float or int.");
                    break;
            }

            EditorGUI.EndProperty();
        }
    }
}

#endif