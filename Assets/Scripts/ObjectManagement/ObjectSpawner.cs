using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.GridSystem;
using FlavorfulStory.ResourceContainer;
using FlavorfulStory.Saving;
using GD.MinMaxSlider;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace FlavorfulStory.ObjectManagement
{
    /// <summary> Спавнер объектов на сцене. </summary>
    [RequireComponent(typeof(SaveableEntity))]
    public class ObjectSpawner : MonoBehaviour, ISaveable
    {
        #region Fields and Properties

        /// <summary> Префаб объекта для спавна. </summary>
        [Header("Настройки спавна")]
        [Tooltip("Префаб объекта для спавна."), SerializeField]
        protected GameObject _spawnObjectPrefab;

        /// <summary> Количество объектов для спавна. </summary>
        [Tooltip("Количество объектов для спавна."), SerializeField, Range(1f, 100f)]
        private int _spawnObjectsNumber;

        /// <summary> Диапазон масштабирования объектов при спавне. </summary>
        [Tooltip("Диапазон масштабирования объектов при спавне."), SerializeField, MinMaxSlider(0.5f, 2f)]
        private Vector2 _scaleVariation;

        /// <summary> Ширина зоны спавна. </summary>
        [Header("Размер зоны спавна")]
        [Tooltip("Ширина зоны спавна."), SerializeField, Range(0f, 1000f)]
        private int _widthSpawnArea;

        /// <summary> Длина зоны спавна. </summary>
        [Tooltip("Длина зоны спавна."), SerializeField, Range(0f, 1000f)]
        private int _lengthSpawnArea;

        // <summary> Минимальное расстояние между заспавненными объектами. </summary>
        [Header("Распределение объектов")]
        [Tooltip("Минимальное расстояние между заспавненными объектами."), SerializeField, Range(1f, 50f)]
        public int _minSpacing;

        /// <summary> Слой препятствий, с которыми объекты не должны пересекаться. </summary>
        [Header("Основные настройки спавна")]
        [Tooltip("Слой препятствий, с которыми объекты не должны пересекаться."), SerializeField]
        private LayerMask _obstaclesLayerMask;

        /// <summary> Максимальное количество попыток спавна. </summary>
        [Tooltip("Максимальное количество попыток спавна."), SerializeField, Range(100, 10000)]
        private int _maxIterations;

        /// <summary> Цвет визуализации зоны спавна. </summary>
        [Tooltip("Цвет визуализации зоны спавна."), SerializeField]
        private Color _spawnAreaVisualizationColor;

        /// <summary> Список всех заспавненных объектов. </summary>
        protected readonly List<GameObject> _spawnedObjects = new();

        /// <summary> Контейнер зависимостей Zenject. </summary>
        private DiContainer _container;

        /// <summary> Провайдер позиции на гриде. </summary>
        private GridPositionProvider _gridPositionProvider;

        #endregion

        /// <summary> Внедрить контейнер зависимостей. </summary>
        /// <param name="container"> Контейнер Zenject. </param>
        /// <param name="gridPositionProvider"> </param>
        [Inject]
        private void Construct(DiContainer container, GridPositionProvider gridPositionProvider)
        {
            _container = container;
            _gridPositionProvider = gridPositionProvider;
        }

        /// <summary> Запуск процесса спавна при старте сцены. </summary>
        private void Start()
        {
            if (_spawnedObjects.Count == 0) SpawnObjects();
        }

        /// <summary> Спавнит объекты случайным образом в заданной зоне спавна. </summary>
        private void SpawnObjects()
        {
            int iterations = 0;
            while (_spawnedObjects.Count < _spawnObjectsNumber && iterations < _maxIterations)
            {
                var randWorld = GetRandomWorldPosition(transform.position);
                var gridPos = _gridPositionProvider.WorldToGrid(randWorld);
                var snappedPos = _gridPositionProvider.GetCellCenterWorld(gridPos);

                if (CanSpawnAtPosition(snappedPos)) SpawnObject(snappedPos, Random.Range(0, 360f), GetRandomScale());

                iterations++;
            }

            if (_spawnedObjects.Count != _spawnObjectsNumber)
                Debug.LogError($"Не удалось заспавнить все объекты: {name}");
        }

        /// <summary> Спавнит объект в заданной позиции с заданными параметрами масштаба и вращения. </summary>
        /// <param name="position"> Позиция спавна объекта. </param>
        /// <param name="rotationY"> Угол вращения объекта по оси Y. </param>
        /// <param name="scale"> Масштаб объекта. </param>
        /// <param name="data"> Дополнительные данные. </param>
        protected virtual GameObject SpawnObject(Vector3 position, float rotationY, Vector3 scale, object data = null)
        {
            var instance = _container.InstantiatePrefab(_spawnObjectPrefab, position,
                Quaternion.Euler(0, rotationY, 0), transform);
            instance.transform.localScale = scale;
            _spawnedObjects.Add(instance);

            if (instance.TryGetComponent(out IDestroyable destroyable))
                destroyable.OnObjectDestroyed += RemoveObjectFromList;

            return instance;
        }

        /// <summary> Удаляет объект из списка заспавненных объектов. </summary>
        /// <param name="destroyable"> Объект, который необходимо удалить из списка. </param>
        private void RemoveObjectFromList(IDestroyable destroyable)
        {
            if (destroyable is not MonoBehaviour monoBehaviour) return;

            destroyable.OnObjectDestroyed -= RemoveObjectFromList;
            _spawnedObjects.Remove(monoBehaviour.gameObject);
        }

        /// <summary> Генерирует случайную точку в пределах зоны спавна. </summary>
        /// <param name="center"> Центр зоны спавна. </param>
        /// <returns> Случайная точка в пределах зоны спавна. </returns>
        private Vector3 GetRandomWorldPosition(Vector3 center) => new(
            Random.Range(center.x - _widthSpawnArea * 0.5f, center.x + _widthSpawnArea * 0.5f),
            0f,
            Random.Range(center.z - _lengthSpawnArea * 0.5f, center.z + _lengthSpawnArea * 0.5f)
        );

        /// <summary> Получает случайный коэффициент масштабирования. </summary>
        /// <returns> Коэффициент масштабирования в виде вектора. </returns>
        private Vector3 GetRandomScale() => Vector3.one * Random.Range(_scaleVariation.x, _scaleVariation.y);

        /// <summary> Проверяет возможность спавна объекта в заданной позиции. </summary>
        /// <param name="position"> Позиция для проверки. </param>
        /// <returns> Возвращает true, если объект может быть заспавнен в данной позиции, иначе false. </returns>
        private bool CanSpawnAtPosition(Vector3 position) => Physics.OverlapSphere(
            position, _minSpacing, _obstaclesLayerMask, QueryTriggerInteraction.Collide
        ).Length == 0;

        #region Saving

        /// <summary> Структура для записи состояния заспавненных объектов. </summary>
        [Serializable]
        protected struct SpawnedObjectRecord
        {
            /// <summary> Позиция заспавненного объекта. </summary>
            public SerializableVector3 Position;

            /// <summary> Поворот по оси Y. </summary>
            public float RotationY;

            /// <summary> Размер. </summary>
            public float Scale;
        }

        /// <summary> Фиксация состояния объекта при сохранении. </summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
        public virtual object CaptureState() => _spawnedObjects.Select(spawnedObject => new SpawnedObjectRecord
        {
            Position = new SerializableVector3(spawnedObject.transform.position),
            RotationY = spawnedObject.transform.eulerAngles.y,
            Scale = spawnedObject.transform.localScale.x
        }).ToList();

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public virtual void RestoreState(object state)
        {
            if (_spawnedObjects != null && _spawnedObjects.Count != 0) DestroySpawnedObjects();

            var spawnedObjectRecords = state as List<SpawnedObjectRecord>;
            SpawnFromSave(spawnedObjectRecords);
        }

        /// <summary> Уничтожить заспавненные объекты. </summary>
        protected void DestroySpawnedObjects()
        {
            foreach (var spawnedObject in _spawnedObjects) Destroy(spawnedObject);

            _spawnedObjects.Clear();
        }

        /// <summary> Восстанавливает заспавненные объекты из сохраненного состояния. </summary>
        /// <param name="records"> Список сохраненных объектов. </param>
        private void SpawnFromSave(List<SpawnedObjectRecord> records)
        {
            foreach (var record in records)
                SpawnObject(record.Position.ToVector(), record.RotationY, Vector3.one * record.Scale);
        }

        #endregion

        #region Debug

#if UNITY_EDITOR
        /// <summary> Валидация данных. </summary>
        /// <remarks> Коллбэк из UnityAPI. </remarks>
        private void OnValidate()
        {
            if (_spawnObjectPrefab.GetComponent<IHitable>() != null)
                Debug.LogError(
                    "В конфиге спавнера не должен находится объект, реализующий интерфейс IHitable. " +
                    $"Используйте DestroyableContainerSpawner для {name}");
        }

        /// <summary> Отображает визуализацию зоны спавна в редакторе. </summary>
        private void OnDrawGizmosSelected()
        {
            var color = new Color(_spawnAreaVisualizationColor.r, _spawnAreaVisualizationColor.g,
                _spawnAreaVisualizationColor.b, 0.3f);
            Gizmos.color = color;

            Gizmos.DrawCube(transform.position, new Vector3(_widthSpawnArea, 0f, _lengthSpawnArea));
        }
#endif

        #endregion
    }
}