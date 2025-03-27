using UnityEngine;

namespace FlavorfulStory.Control.CursorSystem
{
    /// <summary> ScriptableObject, содержащий настройки курсоров. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Cursor Data")]
    public class CursorData : ScriptableObject
    {
        /// <summary> Массив сопоставлений типов курсора и их текстур. </summary>
        [SerializeField] private CursorMapping[] _cursorMappings;

        /// <summary> Синглтон-экземпляр CursorData. </summary>
        private static CursorData _instance;

        /// <summary> Получить экземпляр CursorData из ресурсов. </summary>
        public static CursorData Instance
        {
            get
            {
                if (_instance) return _instance;

                _instance = Resources.Load<CursorData>("CursorData");
                if (!_instance) Debug.LogError("CursorData asset not found!");
                return _instance;
            }
        }

        /// <summary> Получить сопоставление курсора по его типу. </summary>
        /// <param name="type"> Тип курсора. </param>
        /// <returns> Сопоставление курсора или значение по умолчанию. </returns>
        public CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (var mapping in _cursorMappings)
                if (mapping.Type == type)
                    return mapping;
            return _cursorMappings[0];
        }
    }
}