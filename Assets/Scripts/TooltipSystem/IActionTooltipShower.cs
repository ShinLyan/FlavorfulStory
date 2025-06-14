using FlavorfulStory.InteractionSystem;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Интерфейс для отображения тултипа возможных действий взаимодействия с интерактивным объектом. </summary>
    public interface IActionTooltipShower
    {
        /// <summary> Показать тултип действий для указанного объекта взаимодействия. </summary>
        /// <param name="interactable"> Объект, с которым можно взаимодействовать. </param>
        public void Show(IInteractable interactable);

        /// <summary> Скрыть текущий отображаемый тултип. </summary>
        public void Hide();
    }
}