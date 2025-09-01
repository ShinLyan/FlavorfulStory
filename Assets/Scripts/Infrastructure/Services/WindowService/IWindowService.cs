using System;
using FlavorfulStory.UI.Windows;

namespace FlavorfulStory.Infrastructure.Services.WindowService
{
    /// <summary> Сервис управления UI-окнами: открытие, закрытие, события, пауза и ввод. </summary>
    public interface IWindowService
    {
        /// <summary> Возвращает экземпляр окна по типу. Создает при первом вызове. </summary>
        TWindow GetWindow<TWindow>() where TWindow : BaseWindow;

        /// <summary> Открывает окно. Если уже открыто — переносит наверх. </summary>
        void OpenWindow<TWindow>() where TWindow : BaseWindow;

        /// <summary> Закрывает указанное окно. </summary>
        void CloseWindow<TWindow>() where TWindow : BaseWindow;

        /// <summary> Закрывает самое верхнее (последнее) окно. </summary>
        void CloseTopWindow();

        /// <summary> Событие: окно было открыто. </summary>
        event Action<BaseWindow> OnWindowOpened;

        /// <summary> Событие: окно было закрыто. </summary>
        event Action<BaseWindow> OnWindowClosed;

        /// <summary> Есть ли открытые окна. </summary>
        bool HasOpenWindows { get; }
    }
}