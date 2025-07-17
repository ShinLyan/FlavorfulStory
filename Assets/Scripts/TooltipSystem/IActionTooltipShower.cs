namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Интерфейс для отображения тултипа возможных действий взаимодействия с интерактивным объектом. </summary>
    public interface IActionTooltipShower
    {
        /// <summary> Добавляет действие во всплывающую подсказку. </summary>
        /// <param name="action"> Данные действия (клавиша + описание). </param>
        void Add(ActionTooltipData action);

        /// <summary> Удаляет действие из тултипа. </summary>
        /// <param name="action"> Данные действия, которые нужно удалить. </param>
        void Remove(ActionTooltipData action);
    }
}