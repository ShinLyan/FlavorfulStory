using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure.Adresses;
using FlavorfulStory.UI.Windows;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Infrastructure.Factories.Window
{
    /// <summary> Фабрика для создания UI-окон с DI и прогревом. </summary>
    public class WindowFactory : IWindowFactory
    {
        /// <summary> Канвас, в котором будут появляться окна. </summary>
        private readonly Canvas _canvas;
        /// <summary> ScriptableObject с адресами всех окон. </summary>
        private readonly WindowAdresses _windows;
        /// <summary> DI-контейнер Zenject для внедрения зависимостей. </summary>
        private readonly DiContainer _container;

        /// <summary> Словарь соответствий: тип окна → его префаб. </summary>
        private readonly Dictionary<Type, BaseWindow> _prefabByType = new();
        
        /// <summary> Родитель для всех окон. </summary>
        private Transform _spawnRoot;
        
        /// <summary> Флаг, указывающий, что фабрика уже была прогрета. </summary>
        private bool _isWarmedUp;

        /// <summary> Конструктор фабрики. </summary>
        /// <param name="canvas"> Канвас, куда будут спавниться окна. </param>
        /// <param name="windows"> Адреса префабов окон. </param>
        /// <param name="container"> Контейнер Zenject для внедрения зависимостей. </param>
        public WindowFactory(Canvas canvas, WindowAdresses windows, DiContainer container)
        {
            _canvas = canvas;
            _windows = windows;
            _container = container;
        }

        /// <summary> Устанавливает корень для будущих окон (обычно Canvas). </summary>
        public void Initialize() => _spawnRoot = _canvas.transform;

        /// <summary> Прогревает фабрику, подготавливая словарь типов. </summary>
        public async UniTask WarmUpAsync()
        {
            if (_isWarmedUp) return;

            foreach (var window in EnumerateAllPrefabs())
            {
                if (!window)
                {
                    Debug.LogError($"[{nameof(WindowFactory)}] Prefab[{window}] has not been assigned.");
                    continue;
                }

                var type = window.GetType();
                _prefabByType.TryAdd(type, window);
            }

            _isWarmedUp = true;
            await UniTask.Yield();
        }

        /// <summary> Создает окно по типу, внедряет зависимости, вызывает Initialize() и активирует. </summary>
        public T CreateWindow<T>() where T : BaseWindow
        {
            EnsureWarmup();

            var type = typeof(T);
            if (!_prefabByType.TryGetValue(type, out var prefab))
            {
                Debug.LogError($"[Windows] No prefab found for window type {type.Name}.");
                return null;
            }

            var instance = UnityEngine.Object.Instantiate(prefab.gameObject, _spawnRoot);
            instance.SetActive(false);

            _container.InjectGameObject(instance);

            foreach (var init in instance.GetComponentsInChildren<IInitializable>(true))
                init.Initialize();

            instance.SetActive(true);
            return instance.GetComponent<T>();
        }
        
        /// <summary> Возвращает все окна из ScriptableObject с адресами. </summary>
        private IEnumerable<BaseWindow> EnumerateAllPrefabs()
        {
            if (_windows.ConfirmationWindow) yield return _windows.ConfirmationWindow;
            if (_windows.SummaryWindow) yield return _windows.SummaryWindow;
            if (_windows.RepairableBuildingWindow) yield return _windows.RepairableBuildingWindow;
            if (_windows.InventoryExchangeWindow) yield return _windows.InventoryExchangeWindow;
            if (_windows.GameMenu) yield return _windows.GameMenu;
            if (_windows.SettingsWindow) yield return _windows.SettingsWindow;
            if (_windows.NewGameWindow) yield return _windows.NewGameWindow;
            if (_windows.ExitConfirmationWindow) yield return _windows.ExitConfirmationWindow;
        }

        /// <summary> Защита от преждевременного вызова CreateWindow. </summary>
        private void EnsureWarmup()
        {
            if (_isWarmedUp) return;
            Debug.LogError($"[{nameof(WindowFactory)}] Used before WarmUpAsync(). Warming up on the fly.");
            WarmUpAsync().Forget();
        }
    }
}