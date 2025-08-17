using UnityEngine;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;

namespace FlavorfulStory.InteractionSystem
{
    /// <summary> Интерфейс для объектов, с которыми можно взаимодействовать. </summary>
    public interface IInteractable
    {
        /// <summary> Действие для отображения в тултипе. </summary>
        ActionTooltipData ActionTooltip { get; }

        /// <summary> Доступно ли взаимодействие с объектом в текущий момент? </summary>
        bool IsInteractionAllowed { get; }

        /// <summary> Вычисляет расстояние от объекта до другого указанного трансформа. </summary>
        /// <param name="otherTransform"> Трансформ, до которого нужно измерить расстояние. </param>
        /// <returns> Расстояние до указанного трансформа. </returns>
        float GetDistanceTo(Transform otherTransform);

        /// <summary> Начинает процесс взаимодействия с объектом. </summary>
        /// <param name="player"> Игрок, который начал взаимодействие. </param>
        /// <remarks> Например, отображение UI, анимации, подготовка логики. </remarks>
        void BeginInteraction(PlayerController player);

        /// <summary> Завершает взаимодействие. </summary>
        /// <param name="player"> Игрок, завершивший взаимодействие. </param>
        /// <remarks> Например, закрытие UI, сброс состояний, завершение анимаций. </remarks>
        void EndInteraction(PlayerController player);
        
        /// <summary> Завершает взаимодействие. </summary>
        /// <param name="player"> Игрок, завершивший взаимодействие. </param>
        /// <remarks> Например, закрытие UI, сброс состояний, завершение анимаций. </remarks>
        void OnInteractionTriggerEnter() {}

        /// <summary> Завершает взаимодействие. </summary>
        /// <param name="player"> Игрок, завершивший взаимодействие. </param>
        /// <remarks> Например, закрытие UI, сброс состояний, завершение анимаций. </remarks>
        void OnInteractionTriggerExit() { }
    }
}