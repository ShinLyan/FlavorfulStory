using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.Windows.UI;
using UnityEngine;

namespace FlavorfulStory.Windows
{
    /// <summary> Реализация IWindowService: управляет окнами, вводом и временем. </summary>
    public class WindowService : IWindowService, IDisposable
    {
        /// <summary> Фабрика UI-окон. </summary>
        private readonly IWindowFactory _windowFactory;

        /// <summary> Зарегистрированные окна (по типу). </summary>
        private readonly Dictionary<Type, BaseWindow> _windows = new();

        /// <summary> Список открытых окон в порядке их открытия. </summary>
        private readonly List<BaseWindow> _openedWindows = new();

        /// <summary> Есть ли хотя бы одно открытое окно. </summary>
        public bool HasOpenWindows => _openedWindows.Count > 0;

        /// <summary> Событие: окно открылось. </summary>
        public event Action<BaseWindow> WindowOpened;

        /// <summary> Событие: окно закрылось. </summary>
        public event Action<BaseWindow> WindowClosed;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="windowFactory"> Фабрика окон. </param>
        public WindowService(IWindowFactory windowFactory) => _windowFactory = windowFactory;

        /// <summary> Возвращает окно по типу. Создает, если ещё не зарегистрировано. </summary>
        public TWindow GetWindow<TWindow>() where TWindow : BaseWindow
        {
            if (_windows.TryGetValue(typeof(TWindow), out var baseWindow)) return baseWindow as TWindow;

            var window = _windowFactory.CreateWindow<TWindow>();
            if (!window) return null;

            window.SetActive(false);
            TryAddWindow(window);
            return window;
        }

        /// <summary> Регистрирует окно, добавляя обработчики событий открытия/закрытия. </summary>
        private void TryAddWindow<TWindow>(TWindow window) where TWindow : BaseWindow
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
                WindowOpened?.Invoke(window);
            };

            window.Closed += () =>
            {
                _openedWindows.Remove(window);
                if (_openedWindows.Count == 0)
                {
                    WorldTime.Unpause();
                    InputWrapper.UnblockAllInput();
                }

                WindowClosed?.Invoke(window);
            };
        }

        /// <summary> Открывает окно. Если уже открыто — переносит наверх. </summary>
        public void OpenWindow<TWindow>() where TWindow : BaseWindow
        {
            var window = GetWindow<TWindow>();
            if (!window) Debug.LogError("Window not found: " + typeof(TWindow).Name);

            if (window.IsOpened)
                BringWindowToFront(window);
            else
                window.Open();
        }

        /// <summary> Поднять окно на верх стека. </summary>
        /// <param name="window"> Окно, которое нужно поднять. </param>
        private void BringWindowToFront(BaseWindow window)
        {
            _openedWindows.Remove(window);
            _openedWindows.Add(window);
            window.transform.SetAsLastSibling();
        }

        /// <summary> Закрывает указанное окно. </summary>
        public void CloseWindow<TWindow>() where TWindow : BaseWindow
        {
            var window = GetWindow<TWindow>();
            if (!window) return;

            window.Close();
        }

        /// <summary> Закрывает самое верхнее (последнее) открытое окно. </summary>
        public void CloseTopWindow()
        {
            if (_openedWindows.Count == 0) return;

            var top = _openedWindows.Last();
            top.Close();
        }

        /// <summary> Очищает события. Вызывать при разрушении/сцене. </summary>
        public void Dispose()
        {
            WindowOpened = null;
            WindowClosed = null;
        }
    }
}