using UnityEngine;

namespace FlavorfulStory.Control.CursorSystem
{
    /// <summary> Контроллер для управления типом курсора. </summary>
    public static class CursorController
    {
        /// <summary> Установить курсор соответствующего типа. </summary>
        /// <param name="type"> Тип курсора, который должен быть установлен. </param>
        public static void SetCursor(CursorType type)
        {
            var mapping = CursorData.Instance.GetCursorMapping(type);
            Cursor.SetCursor(mapping.Texture, mapping.Hotspot, CursorMode.Auto);
        }
    }
}