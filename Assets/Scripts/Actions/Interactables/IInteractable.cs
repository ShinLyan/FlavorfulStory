using FlavorfulStory.TooltipSystem;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Интерфейс для объектов, с которыми можно взаимодействовать. </summary>
    public interface IInteractable : ITooltipable
    {
        /// <summary> Проверяет, доступно ли взаимодействие с объектом. </summary>
        /// <returns> Возвращает true, если взаимодействие разрешено. </returns>
        public bool IsInteractionAllowed { get; set; }

        public bool IsBlockingMovement{ get; set; }
        
        /// <summary> Выполняет взаимодействие с объектом. </summary>
        public void Interact();

        /// <summary> Вычисляет расстояние до указанного трансформа. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого измеряется расстояние. </param>
        /// <returns> Расстояние до указанного трансформа. </returns>
        public float GetDistanceTo(Transform otherTransform);
    }
}