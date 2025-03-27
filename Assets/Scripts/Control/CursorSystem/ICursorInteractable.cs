namespace FlavorfulStory.Control.CursorSystem
{
    /// <summary> Интерфейс для объектов, с которыми можно взаимодействовать с помощью курсора. </summary>
    public interface ICursorInteractable
    {
        /// <summary> Возвращает тип курсора, который должен отображаться при наведении на объект. </summary>
        /// <returns> Тип курсора (например, указатель, рука и т.д.). </returns>
        CursorType GetCursorType();

        /// <summary> Обрабатывает взаимодействие с объектом при наведении курсора и действии игрока. </summary>
        /// <param name="controller"> Контроллер игрока, выполняющий взаимодействие. </param>
        /// <returns> True, если взаимодействие успешно обработано. </returns>
        bool HandleCursorInteraction(PlayerController controller);
    }
}