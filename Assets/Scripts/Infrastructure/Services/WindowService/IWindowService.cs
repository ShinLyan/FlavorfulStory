using System;

namespace FlavorfulStory
{
    public interface IWindowService
    {
        TWindow GetWindow<TWindow>() where TWindow : BaseWindow;
        TWindow OpenWindow<TWindow>() where TWindow : BaseWindow;
        void CloseWindow<TWindow>() where TWindow : BaseWindow;
        void CloseTopWindow();
        void CloseAllWindows();

        void TryAddWindow<TWindow>(TWindow window) where TWindow : BaseWindow;
        
        event Action<BaseWindow> OnWindowOpened;
        event Action<BaseWindow> OnWindowClosed;

        bool IsOpened<TWindow>() where TWindow : BaseWindow;
        bool HasOpenWindows { get; }
    }
}