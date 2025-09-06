using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.ObjectManagement
{
    /// <summary> Переключатель объектов. </summary>
    public class ObjectSwitcher : MonoBehaviour
    {
        /// <summary> Префабы грейдов одного объекта. </summary>
        /// <remarks> Версия по умолчанию (первый ребенок в иерархии) добавляется автоматически. </remarks>
        [Tooltip("Префабы всех грейдов данного объекта, кроме начального"), SerializeField]
        private GameObject[] _objectPrefabs;

        /// <summary> Объекты на сцене, представляющие грейды разного уровня. </summary>
        private List<GameObject> _spawnedObjects;

        /// <summary> DI контейнер Zenject. </summary>
        private DiContainer _container;

        /// <summary> Получить количество грейдов. </summary>
        public int ObjectsCount => _objectPrefabs.Length + 1;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="container"> DI контейнер Zenject. </param>
        [Inject]
        private void Construct(DiContainer container) => _container = container;

        /// <summary> Инициализировать все грейды. </summary>
        /// <remarks> В качестве первого грейда выступает дочерний объект под индексом 0.
        /// Многократный вызов не приведет к ошибке. </remarks>
        public void Initialize()
        {
            _spawnedObjects = new List<GameObject>(_objectPrefabs.Length + 1) { transform.GetChild(0).gameObject };
            foreach (var prefab in _objectPrefabs)
            {
                var spawned = _container.InstantiatePrefab(prefab, transform);
                spawned.SetActive(false);
                _spawnedObjects.Add(spawned);
            }
        }

        /// <summary> Переключится на выбранный грейд. </summary>
        /// <remarks> Включает одного из детей данного Gameobject'а. Выключает остальные. </remarks>
        /// <param name="index"> Индекс грейда. </param>
        public void SwitchTo(int index)
        {
            for (int i = 0; i < _spawnedObjects.Count; i++) _spawnedObjects[i].SetActive(i == index);
        }
    }
}