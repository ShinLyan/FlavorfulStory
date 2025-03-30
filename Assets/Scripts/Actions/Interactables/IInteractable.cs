using FlavorfulStory.TooltipSystem;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Интерфейс для объектов, с которыми можно взаимодействовать. </summary>
    public interface IInteractable : ITooltipable
    {
        /// <summary> Доступно ли взаимодействие с объектом? </summary>
        /// <returns> Возвращает true, если взаимодействие разрешено. </returns>
        bool IsInteractionAllowed { get; }

        /// <summary> Выполняет взаимодействие с объектом. </summary>
        void Interact();

        /// <summary> Вычисляет расстояние до указанного трансформа. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого измеряется расстояние. </param>
        /// <returns> Расстояние до указанного трансформа. </returns>
        float GetDistanceTo(Transform otherTransform);
    }
}