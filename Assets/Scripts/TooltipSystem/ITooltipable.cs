using UnityEngine;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Объект, предоставляющий данные для тултипа. </summary>
    public interface ITooltipable
    {
        /// <summary> Возвращает заголовок тултипа. </summary>
        /// <returns> Строка с заголовком тултипа. </returns>
        string TooltipTitle { get; }

        /// <summary> Возвращает описание тултипа. </summary>
        /// <returns> Строка с описанием тултипа. </returns>
        string TooltipDescription { get; }

        /// <summary> Возвращает мировую позицию объекта. </summary>
        /// <returns> Вектор позиции объекта в мировых координатах. </returns>
        Vector3 WorldPosition { get; }
    }
}