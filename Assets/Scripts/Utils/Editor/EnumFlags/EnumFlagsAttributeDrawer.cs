using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.Utils.Editor.EnumFlags
{
    /// <summary> Кастомный отрисовщик для атрибута <see cref="EnumFlagsAttribute"/>. </summary>
    /// <remarks> Позволяет редактировать перечисления с флагами в виде MaskField в инспекторе. </remarks>
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        /// <summary> Отрисовать кастомное поле в инспекторе для enum с флагами. </summary>
        /// <param name="position"> Область, где будет отрисовываться поле. </param>
        /// <param name="property"> Свойство, которое редактируется (enum с флагами). </param>
        /// <param name="label"> Название свойства, отображаемое в инспекторе. </param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int currentValue = property.intValue;

            // Отобразить мультиселект (MaskField) для выбора нескольких значений
            int newValue = EditorGUI.MaskField(position, label, currentValue, property.enumNames);

            // Установить новое значение с учетом выбора всех/никаких значений
            property.intValue = newValue switch
            {
                ~0 => ~0, // Если выбраны все значения
                0 => 0, // Если ничего не выбрано
                _ => newValue
            };
        }
    }
}