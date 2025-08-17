using System;
using JetBrains.Annotations;

namespace FlavorfulStory
{
    public interface IWindowService
    {
        [CanBeNull] TWindow GetWindow<TWindow>() where TWindow : BaseWindow;
        [CanBeNull] TWindow OpenWindow<TWindow>() where TWindow : BaseWindow;
        void CloseWindow<TWindow>() where TWindow : BaseWindow;
        void CloseAllWindows();

        void TryAddWindow<TWindow>(TWindow window) where TWindow : BaseWindow;

        // Вместо ReactiveCommand — обычные события.
        event Action<BaseWindow> OnWindowOpened;
        event Action<BaseWindow> OnWindowClosed;

        bool IsOpened<TWindow>() where TWindow : BaseWindow;
    }
}