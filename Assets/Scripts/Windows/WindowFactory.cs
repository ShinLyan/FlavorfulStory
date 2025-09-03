using System;
using System.Collections.Generic;
using FlavorfulStory.Windows.UI;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace FlavorfulStory.Windows
{
    /// <summary> Фабрика для создания UI-окон с DI и прогревом. </summary>
    public class WindowFactory : IInitializable, IWindowFactory
    {
        /// <summary> Родитель для всех окон. </summary>
        private readonly Transform _spawnRoot;

        /// <summary> ScriptableObject с адресами всех окон. </summary>
        private readonly WindowAddresses _windows;

        /// <summary> DI-контейнер Zenject для внедрения зависимостей. </summary>
        private readonly DiContainer _container;

        /// <summary> Словарь соответствий: тип окна → его префаб. </summary>
        private readonly Dictionary<Type, BaseWindow> _prefabByType = new();

        /// <summary> Конструктор фабрики. </summary>
        /// <param name="canvas"> Канвас, куда будут спавниться окна. </param>
        /// <param name="windows"> Адреса префабов окон. </param>
        /// <param name="container"> Контейнер Zenject для внедрения зависимостей. </param>
        public WindowFactory(Canvas canvas, WindowAddresses windows, DiContainer container)
        {
            _spawnRoot = canvas.transform;
            _windows = windows;
            _container = container;
        }

        /// <summary> Инициализирует фабрику: устанавливает родитель и кэширует все доступные префабы окон. </summary>
        public void Initialize()
        {
            foreach (var window in _windows.AllWindows())
            {
                if (!window)
                {
                    Debug.LogError($"[{nameof(WindowFactory)}] Prefab[{window}] has not been assigned.");
                    continue;
                }

                _prefabByType.TryAdd(window.GetType(), window);
            }
        }

        /// <summary> Создает окно по типу, внедряет зависимости, вызывает Initialize() и активирует. </summary>
        /// <typeparam name="T"> Тип окна, унаследованный от <see cref="BaseWindow"/>. </typeparam>
        /// <returns> Экземпляр созданного окна, либо null, если префаб не найден. </returns>
        public T CreateWindow<T>() where T : BaseWindow
        {
            var type = typeof(T);
            if (!_prefabByType.TryGetValue(type, out var prefab))
            {
                Debug.LogError($"[Windows] No prefab found for window type {type.Name}.");
                return null;
            }

            var instance = Object.Instantiate(prefab.gameObject, _spawnRoot);
            instance.SetActive(false);

            _container.InjectGameObject(instance);

            foreach (var initializable in instance.GetComponentsInChildren<IInitializable>(true))
                initializable.Initialize();

            instance.SetActive(true);
            return instance.GetComponent<T>();
        }
    }
}