using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem;
using UnityEngine;

namespace FlavorfulStory.InteractionSystem
{
    /// <summary> Интерфейс для объектов, с которыми можно взаимодействовать
    /// (например, двери, предметы, NPC). </summary>
    /// <remarks> Наследуется от <see cref="ITooltipableAction"/>, что позволяет отображать всплывающие
    /// подсказки при наведении. </remarks>
    public interface IInteractable : ITooltipableAction
    {
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
    }
}