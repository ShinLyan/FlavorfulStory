using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    public class WindowFactory : IWindowFactory
    {
        private readonly Canvas _canvas;
        private readonly WindowAdresses _windows;
        private readonly DiContainer _container;

        private readonly Dictionary<Type, BaseWindow> _prefabByType = new();
        private Transform _spawnRoot;
        private bool _isWarmedUp;

        public WindowFactory(Canvas canvas, WindowAdresses windows, DiContainer container)
        {
            _canvas = canvas;
            _windows = windows;
            _container = container;
        }

        public void Initialize()
        {
            _spawnRoot = _canvas.transform;
        }

        public async UniTask WarmUpAsync()
        {
            if (_isWarmedUp) return;

            foreach (var go in EnumerateAllPrefabs())
            {
                if (!go)
                {
                    Debug.LogError($"[{nameof(WindowFactory)}] Null prefab in WindowAdresses.");
                    continue;
                }

                var window = go.GetComponent<BaseWindow>();
                if (!window)
                {
                    Debug.LogError($"[{nameof(WindowFactory)}] Prefab {go.name} has no BaseWindow component.");
                    continue;
                }

                var type = window.GetType();
                _prefabByType.TryAdd(type, window);
            }

            _isWarmedUp = true;
            await UniTask.Yield();
        }

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
        
        private IEnumerable<GameObject> EnumerateAllPrefabs()
        {
            // Используем текущую структуру WindowAddresses с явными полями (минимальные изменения)
            // Тут должны быть все окна
            
            // TODO: Актуализировать. Сначала проверка, затем yield return
            if (_windows.TestWindow) yield return _windows.TestWindow;
        }

        private void EnsureWarmup()
        {
            if (_isWarmedUp) return;
            Debug.LogError($"[{nameof(WindowFactory)}] Used before WarmUpAsync(). Warming up on the fly.");
            WarmUpAsync().Forget();
        }
    }
}