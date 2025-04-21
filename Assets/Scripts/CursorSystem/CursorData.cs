using UnityEngine;

namespace FlavorfulStory.CursorSystem
{
    /// <summary> Настройки курсоров для разных типов взаимодействия. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Cursor Data")]
    public class CursorData : ScriptableObject
    {
        /// <summary> Сопоставления типов курсора и их текстур. </summary>
        /// <remarks> Используется для выбора нужной текстуры курсора в зависимости от ситуации
        /// (наведение на объект, диалог, атака и т.п.). </remarks>
        [field: Tooltip("Сопоставление типов курсоров с их текстурами и смещением (hotspot)."), SerializeField]
        public CursorMapping[] CursorMappings { get; private set; }
    }
}