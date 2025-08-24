using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory
{
    public class WindowService : IWindowService, IDisposable
    {
        private readonly IWindowFactory _windowFactory;
        private readonly Dictionary<Type, BaseWindow> _windows = new();
        private readonly List<BaseWindow> _openedWindows = new();

        public bool HasOpenWindows => _openedWindows.Count > 0;
        
        public WindowService(IWindowFactory windowFactory) => _windowFactory = windowFactory;

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
            
            if (window.IsOpened)
            {
                _openedWindows.Remove(window);
                _openedWindows.Add(window);
                window.transform.SetAsLastSibling();
            }
            else
            {
                window.Open();
            }

            return window;
        }

        public void CloseWindow<TWindow>() where TWindow : BaseWindow
        {
            var window = GetWindow<TWindow>();
            if (!window) return;
            
            window.Close();
        }

        public void CloseTopWindow()
        {
            if (_openedWindows.Count == 0) return;
    
            var top = _openedWindows.Last();
            top.Close();
        }
        
        public void CloseAllWindows()
        {
            var snapshot = _openedWindows.ToArray();
            foreach (var window in snapshot) window.Close();
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
            if (!_windows.TryAdd(type, window))
            {
                Debug.LogWarning($"[Windows] Window type \"{type.Name}\" is already registered.");
                return;
            }

            window.Opened += () =>
            {
                if (_openedWindows.Count == 0)
                {
                    WorldTime.Pause();
                    InputWrapper.BlockAllInput();
                    InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
                }
                _openedWindows.Remove(window);
                _openedWindows.Add(window);
                OnWindowOpened?.Invoke(window);
            };

            window.Closed += () =>
            {
                _openedWindows.Remove(window);
                if (_openedWindows.Count == 0)
                {
                    WorldTime.Unpause();
                    InputWrapper.UnblockAllInput();
                }
                OnWindowClosed?.Invoke(window);
            };
        }

        public void Dispose()
        {
            OnWindowOpened = null;
            OnWindowClosed = null;
        }
    }
}