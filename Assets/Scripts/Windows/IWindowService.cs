using System;
using FlavorfulStory.Windows.UI;

namespace FlavorfulStory.Windows
{
    /// <summary> Сервис управления UI-окнами: открытие, закрытие, события, пауза и ввод. </summary>
    public interface IWindowService
    {
        /// <summary> Есть ли открытые окна. </summary>
        bool HasOpenWindows { get; }

        /// <summary> Событие открытия окна. </summary>
        event Action WindowOpened;

        /// <summary> Событие закрытия окна. </summary>
        event Action WindowClosed;

        /// <summary> Возвращает экземпляр окна по типу. Создает при первом вызове. </summary>
        TWindow GetWindow<TWindow>() where TWindow : BaseWindow;

        /// <summary> Открывает окно. Если уже открыто — переносит наверх. </summary>
        void OpenWindow<TWindow>() where TWindow : BaseWindow;

        /// <summary> Закрывает указанное окно. </summary>
        void CloseWindow<TWindow>() where TWindow : BaseWindow;

        /// <summary> Закрывает самое верхнее (последнее) окно. </summary>
        void CloseTopWindow();
    }
}