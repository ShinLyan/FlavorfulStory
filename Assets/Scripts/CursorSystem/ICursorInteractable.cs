using FlavorfulStory.Player;

namespace FlavorfulStory.CursorSystem
{
    /// <summary> Интерфейс для объектов, с которыми можно взаимодействовать с помощью курсора. </summary>
    public interface ICursorInteractable
    {
        /// <summary> Возвращает тип курсора, который должен отображаться при наведении на объект. </summary>
        /// <returns> Тип курсора (например, указатель, рука и т.д.). </returns>
        CursorType CursorType { get; }

        /// <summary> Пытается выполнить взаимодействие с объектом при наведении курсора. </summary>
        /// <param name="controller"> Контроллер игрока, выполняющий взаимодействие. </param>
        /// <returns> <c>true</c> — взаимодействие успешно обработано и курсор должен быть заменён на указанный
        /// для этого объекта. <c>false</c> — объект не требует замены курсора в текущем контексте. </returns>
        bool TryInteractWithCursor(PlayerController controller);
    }
}