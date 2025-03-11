using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.ResourceContainer;
using FlavorfulStory.Saving;
using GD.MinMaxSlider;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.ObjectManagement
{
    /// <summary> Спавнер объектов на сцене. </summary>
    [RequireComponent(typeof(SaveableEntity))]
    public class ObjectSpawner : MonoBehaviour, ISaveable
    {
        #region Fields and Properties

        /// <summary> Префаб объекта для спавна. </summary>
        [Header("Настройки спавна")] [Tooltip("Префаб объекта для спавна."), SerializeField]
        protected GameObject _spawnObjectPrefab;

        /// <summary> Количество объектов для спавна. </summary>
        [Tooltip("Количество объектов для спавна."), SerializeField, Range(1f, 100f)]
        private int _spawnObjectsNumber;

        /// <summary> Диапазон масштабирования объектов при спавне. </summary>
        [Tooltip("Диапазон масштабирования объектов при спавне."), SerializeField, MinMaxSlider(0.5f, 2f)]
        private Vector2 _scaleVariation;

        /// <summary> Ширина зоны спавна. </summary>
        [Header("Размер зоны спавна")] [Tooltip("Ширина зоны спавна."), SerializeField, Range(0f, 1000f)]
        private int _widthSpawnArea;

        /// <summary> Длина зоны спавна. </summary>
        [Tooltip("Длина зоны спавна."), SerializeField, Range(0f, 1000f)]
        private int _lengthSpawnArea;

        // <summary> Минимальное расстояние между заспавненными объектами. </summary>
        [Header("Распределение объектов")]
        [Tooltip("Минимальное расстояние между заспавненными объектами."), SerializeField, Range(1f, 50f)]
        public int _minSpacing;

        /// <summary> Равномерное ли распределение объектов по сетке? </summary>
        [Tooltip("Равномерное ли распределение объектов по сетке?"), SerializeField]
        private bool _evenSpread;

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

        #endregion

        /// <summary> Запуск процесса спавна при старте сцены. </summary>
        private void Start() => SpawnFromConfig();

        /// <summary> Спавнит объекты на основе конфигурации. </summary>
        private void SpawnFromConfig()
        {
            if (_spawnedObjects.Count != 0) return;

            if (_evenSpread)
                SpawnEvenSpread();
            else
                SpawnRandomly();
        }

        /// <summary> Спавнит объекты равномерно по сетке. </summary>
        //  Пояснения по формулам см. здесь: https://www.youtube.com/watch?v=rSKMYc1CQHE&t=122s
        private void SpawnEvenSpread()
        {
            int objectsPerRow = (int)Mathf.Sqrt(_spawnObjectsNumber);
            int objectsPerColumn = (_spawnObjectsNumber - 1) / objectsPerRow + 1;
            for (int i = 0; i < _spawnObjectsNumber; i++)
            {
                float x = (i / (float)objectsPerRow - objectsPerColumn / 2f + 0.5f) * _minSpacing;
                float z = (i % objectsPerRow - objectsPerRow / 2f + 0.5f) * _minSpacing;
                var offset = _widthSpawnArea > _lengthSpawnArea ? new Vector3(x, 0, z) : new Vector3(z, 0, x);
                SpawnObject(transform.position + offset, Random.value * 360f, GetRandomScale());
            }
        }

        /// <summary> Спавнит объект в заданной позиции с заданными параметрами масштаба и вращения. </summary>
        /// <param name="position"> Позиция спавна объекта. </param>
        /// <param name="rotationY"> Угол вращения объекта по оси Y. </param>
        /// <param name="scale"> Масштаб объекта. </param>
        /// <param name="data"> Дополнительные данные. </param>
        protected virtual GameObject SpawnObject(Vector3 position, float rotationY, Vector3 scale, object data = null)
        {
            var obj = Instantiate(
                _spawnObjectPrefab, position, Quaternion.Euler(0f, rotationY, 0f), transform
            );
            obj.transform.localScale = scale;
            _spawnedObjects.Add(obj);

            if (obj.TryGetComponent(out IDestroyable destroyable))
                destroyable.OnObjectDestroyed += RemoveObjectFromList;

            return obj;
        }

        /// <summary> Удаляет объект из списка заспавненных объектов. </summary>
        /// <param name="destroyable"> Объект, который необходимо удалить из списка. </param>
        private void RemoveObjectFromList(IDestroyable destroyable)
        {
            if (destroyable is not MonoBehaviour monoBehaviour) return;

            destroyable.OnObjectDestroyed -= RemoveObjectFromList;
            _spawnedObjects.Remove(monoBehaviour.gameObject);
        }

        /// <summary> Получает случайный коэффициент масштабирования. </summary>
        /// <returns> Коэффициент масштабирования в виде вектора. </returns>
        private Vector3 GetRandomScale() => Vector3.one * Random.Range(_scaleVariation.x, _scaleVariation.y);

        /// <summary> Спавнит объекты случайным образом в заданной зоне спвна. </summary>
        private void SpawnRandomly()
        {
            int iterations = 0;
            while (_spawnedObjects.Count < _spawnObjectsNumber && iterations < _maxIterations)
            {
                var position = GetRandomPosition(transform.position);
                if (CanSpawnAtPosition(position)) SpawnObject(position, Random.Range(0, 360f), GetRandomScale());

                iterations++;
            }

            if (_spawnedObjects.Count != _spawnObjectsNumber)
                Debug.LogError("Превышен лимит итераций! Увеличьте зону спавна или уменьшите количество объектов");
        }

        /// <summary> Генерирует случайную точку в пределах зоны спавна. </summary>
        /// <param name="areaCenterPosition"> Центр зоны спавна. </param>
        /// <returns> Случайная точка в пределах зоны спавна. </returns>
        private Vector3 GetRandomPosition(Vector3 areaCenterPosition) => new(
            Random.Range(areaCenterPosition.x - _widthSpawnArea * 0.5f, areaCenterPosition.x + _widthSpawnArea * 0.5f),
            0f,
            Random.Range(areaCenterPosition.z - _lengthSpawnArea * 0.5f, areaCenterPosition.z + _lengthSpawnArea * 0.5f)
        );

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
            foreach (var spawnedObject in _spawnedObjects)
                Destroy(spawnedObject);

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
                Debug.LogError("В конфиге спавнера не должен находится объект, реализующий интерфейс IHitable." +
                               " Используйте DestroyableContainerSpawner.cs");
        }

        /// <summary> Отображает визуализацию зоны спавна в редакторе. </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _spawnAreaVisualizationColor;
            Gizmos.DrawWireCube(transform.position, new Vector3(_widthSpawnArea, 0f, _lengthSpawnArea));
        }
#endif

        #endregion
    }
}