using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.CursorSystem
{
    /// <summary> Контроллер для управления типом курсора. </summary>
    /// <remarks> Загружает данные курсоров из ресурсов и предоставляет метод для установки подходящего курсора
    /// в зависимости от контекста (например, диалог, взаимодействие, атака и т.п.). </remarks>
    public static class CursorController // TODO: Перевести на Zenject
    {
        /// <summary> Словарь курсоров для быстрого доступа. </summary>
        private static readonly Dictionary<CursorType, CursorMapping> _cursors;

        /// <summary> Инициализация словаря курсоров. </summary>
        static CursorController()
        {
            var cursorData = Resources.Load<CursorData>("CursorData");
            if (!cursorData)
            {
                Debug.LogError("CursorData asset не найден!");
                return;
            }

            _cursors = new Dictionary<CursorType, CursorMapping>();
            foreach (var mapping in cursorData.CursorMappings)
                if (!_cursors.TryAdd(mapping.Type, mapping))
                    Debug.LogError($"Дубликат типа курсора: {mapping.Type}");
        }

        /// <summary> Установить курсор соответствующего типа. </summary>
        /// <param name="type"> Тип курсора, который должен быть установлен. </param>
        public static void SetCursor(CursorType type)
        {
            if (_cursors.TryGetValue(type, out var mapping))
                Cursor.SetCursor(mapping.Texture, mapping.Hotspot, CursorMode.Auto);
        }
    }
}