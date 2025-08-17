using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory
{
    public class WindowService : IWindowService, IDisposable
    {
        private readonly IWindowFactory _windowFactory;
        private readonly Dictionary<Type, BaseWindow> _windows = new();

        public WindowService(IWindowFactory windowFactory)
        {
            _windowFactory = windowFactory;
        }

        public event Action<BaseWindow> OnWindowOpened;
        public event Action<BaseWindow> OnWindowClosed;

        public bool IsOpened<TWindow>() where TWindow : BaseWindow
        {
            var window = GetWindow<TWindow>();
            return window && window.IsOpened;
        }

        public TWindow OpenWindow<TWindow>() where TWindow : BaseWindow
        {
            var window = GetWindow<TWindow>();
            if (!window) return null;
            
            window.Open();
            return window;
        }

        public void CloseWindow<TWindow>() where TWindow : BaseWindow
        {
            var window = GetWindow<TWindow>();
            if (!window) return;
            window.Close();
        }

        public void CloseAllWindows()
        {
            foreach (var window in _windows.Values)
                window.Close();
        }

        public TWindow GetWindow<TWindow>() where TWindow : BaseWindow
        {
            if (_windows.TryGetValue(typeof(TWindow), out var baseWindow))
                return baseWindow as TWindow;

            var window = _windowFactory.CreateWindow<TWindow>();
            if (!window) return null;

            window.SetActive(false);
            TryAddWindow(window);
            return window;
        }

        public void TryAddWindow<TWindow>(TWindow window) where TWindow : BaseWindow
        {
            var type = window.GetType();
            if (_windows.ContainsKey(type))
            {
                Debug.LogWarning($"[Windows] Window type \"{type.Name}\" is already registered.");
                return;
            }

            _windows.Add(type, window);
            
            window.Opened += () => OnWindowOpened?.Invoke(window);
            window.Closed += () => OnWindowClosed?.Invoke(window);
        }

        public void Dispose()
        {
            OnWindowOpened = null;
            OnWindowClosed = null;
        }
    }
}