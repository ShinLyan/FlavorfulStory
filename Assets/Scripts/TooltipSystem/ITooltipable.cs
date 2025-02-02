using UnityEngine;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Объект, предоставляющий данные для тултипа. </summary>
    public interface ITooltipable
    {
        /// <summary> Возвращает заголовок тултипа. </summary>
        /// <returns> Строка с заголовком тултипа. </returns>
        public string GetTooltipTitle();

        /// <summary> Возвращает описание тултипа. </summary>
        /// <returns> Строка с описанием тултипа. </returns>
        public string GetTooltipDescription();

        /// <summary> Возвращает мировую позицию объекта. </summary>
        /// <returns> Вектор позиции объекта в мировых координатах. </returns>
        public Vector3 GetWorldPosition();
    }
}