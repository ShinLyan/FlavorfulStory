using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace FlavorfulStory.Utils.Factories
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

        /// <summary> Создать экземпляр префаба с параметрами. </summary>
        /// <param name="prefab"> Префаб для создания (если null — используется prefab по умолчанию). </param>
        /// <param name="position"> Позиция (если null — используется Vector3.zero). </param>
        /// <param name="rotation"> Поворот (если null — используется Quaternion.identity). </param>
        /// <param name="parentTransform"> Родительский трансформ (может быть null). </param>
        /// <returns> Созданный экземпляр префаба. </returns>
        public T Create(T prefab = null, Vector3? position = null, Quaternion? rotation = null,
            Transform parentTransform = null)
        {
            var prefabToCreate = prefab ? prefab : _prefab;
            if (!prefabToCreate)
            {
                Debug.LogError($"[{nameof(PrefabFactory<T>)}] Cannot create instance.");
                return null;
            }

            T instance;

            if (position == null && rotation == null)
            {
                // UI
                instance = _container.InstantiatePrefabForComponent<T>(prefabToCreate, parentTransform);
            }
            else
            {
                // World-space
                var pos = position ?? Vector3.zero;
                var rot = rotation ?? Quaternion.identity;
                instance = _container.InstantiatePrefabForComponent<T>(prefabToCreate, pos, rot, parentTransform);
            }

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