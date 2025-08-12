using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Infrastructure.Factories
{
    /// <summary> Базовый класс фабрики объектов, поддерживающий создание и уничтожение компонентов. </summary>
    public class PrefabFactory<T> : IPrefabFactory<T> where T : Component
    {
        /// <summary> Контейнер зависимостей Zenject. </summary>
        private readonly DiContainer _container;

        /// <summary> Префаб компонента для создания. </summary>
        private readonly T _prefab;

        /// <summary> Список созданных экземпляров компонента. </summary>
        private readonly List<T> _spawnedObjects = new();

        /// <summary> Создать экземпляр фабрики. </summary>
        /// <param name="container"> Контейнер зависимостей. </param>
        /// <param name="prefab"> Префаб компонента. </param>
        protected PrefabFactory(DiContainer container, T prefab = null)
        {
            _container = container;
            _prefab = prefab;
        }

        /// <summary> Создать экземпляр префаба. </summary>
        /// <param name="parent"> Родительский трансформ (необязательно). </param>
        /// <returns> Созданный экземпляр префаба. </returns>
        public virtual T Create(Transform parent = null) => Create(_prefab, parent);

        /// <summary> Создать экземпляр префаба. </summary>
        /// <param name="prefab"> Префаб, который нужно создать. </param>
        /// <param name="parent"> Родительский трансформ (необязательно). </param>
        /// <returns> Созданный экземпляр префаба. </returns>
        public virtual T Create(T prefab, Transform parent = null)
        {
            var instance = _container.InstantiatePrefabForComponent<T>(prefab, parent);
            _spawnedObjects.Add(instance);
            return instance;
        }

        /// <summary> Удалить ранее созданный экземпляр префаба. </summary>
        /// <param name="prefab"> Префаб для удаления. </param>
        public virtual void Despawn(T prefab)
        {
            if (_spawnedObjects.Remove(prefab)) Object.Destroy(prefab.gameObject);
        }
    }
}