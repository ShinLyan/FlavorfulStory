using FlavorfulStory.Control;
using FlavorfulStory.TooltipSystem;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Интерфейс для объектов, с которыми можно взаимодействовать
    /// (например, двери, предметы, NPC). </summary>
    /// <remarks> Наследуется от <see cref="ITooltipable"/>, что позволяет отображать всплывающие
    /// подсказки при наведении. </remarks>
    public interface IInteractable : ITooltipable
    {
        /// <summary> Доступно ли взаимодействие с объектом в текущий момент? </summary>
        /// <returns> True, если объект доступен для взаимодействия; иначе — False. </returns>
        bool IsInteractionAllowed { get; }

        /// <summary> Вычисляет расстояние от объекта до другого указанного трансформа. </summary>
        /// <param name="otherTransform"> Трансформ, до которого нужно измерить расстояние. </param>
        /// <returns> Расстояние до указанного трансформа. </returns>
        float GetDistanceTo(Transform otherTransform);

        /// <summary> Начинает процесс взаимодействия с объектом
        /// (например, отображение UI, анимации, подготовка логики). </summary>
        /// <param name="player"> Игрок, который начал взаимодействие. </param>
        void BeginInteraction(PlayerController player);

        /// <summary> Выполняет основное действие взаимодействия
        /// (например, открыть дверь, взять предмет, запустить диалог). </summary>
        /// <param name="player"> Игрок, который взаимодействует. </param>
        void Interact(PlayerController player);

        /// <summary> Завершает взаимодействие (например, закрытие UI, сброс состояний, завершение анимаций). </summary>
        /// <param name="player"> Игрок, завершивший взаимодействие. </param>
        void EndInteraction(PlayerController player);
    }
}