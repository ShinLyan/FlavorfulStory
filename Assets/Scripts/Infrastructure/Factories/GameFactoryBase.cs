using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Infrastructure.Factories
{
    /// <summary> Базовый класс фабрики объектов, поддерживающий создание и уничтожение компонентов. </summary>
    public abstract class GameFactoryBase<T> : IGameFactory<T> where T : Component
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
        protected GameFactoryBase(DiContainer container, T prefab)
        {
            _container = container;
            _prefab = prefab;
        }

        /// <summary> Создать новый экземпляр компонента. </summary>
        /// <param name="parent"> Родительский трансформ (необязательно). </param>
        /// <returns> Созданный экземпляр компонента. </returns>
        public virtual T Create(Transform parent = null)
        {
            var instance = _container.InstantiatePrefabForComponent<T>(_prefab, parent);
            _spawnedObjects.Add(instance);
            return instance;
        }

        /// <summary> Удалить ранее созданный экземпляр компонента. </summary>
        /// <param name="obj"> Объект для удаления. </param>
        public virtual void Despawn(T obj)
        {
            if (_spawnedObjects.Remove(obj)) Object.Destroy(obj.gameObject);
        }
    }
}