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
    /// <summary> Спавнер объектов. </summary>
    public class ObjectSpawner : MonoBehaviour, ISaveable
    {
        #region Fields

        /// <summary> Конфигурация для спавнера объектов, содержащая параметры для спавна. </summary>
        [Tooltip("Конфигурация для спавнера объектов, содержащая параметры для спавна."), SerializeField]
        protected ObjectSpawnerConfig _config;

        /// <summary> Слой, на котором находятся препятствия, с которыми не должны пересекаться объекты. </summary>
        [Tooltip("Слой, на котором находятся препятствия, с которыми не должны пересекаться объекты."), SerializeField]
        private LayerMask _obstaclesLayerMask;

        /// <summary> Если true, будет отображаться область конфигурации для спавна в редакторе. </summary>
        [Tooltip("Если true, будет отображаться область конфигурации для спавна в редакторе."), SerializeField]
        private bool _visualizeConfigArea;

        /// <summary> Цвет для визуализации области спавна в редакторе. </summary>
        [Tooltip("Цвет для визуализации области спавна в редакторе."), SerializeField]
        private Color _gizmosColor;

        /// <summary> Максимальное количество попыток спавна объектов. </summary>
        [Tooltip("Максимальное количество попыток спавна объектов."), SerializeField]
        private int _maxIterations;

        /// <summary> Вариация масштаба объектов при спавне
        /// (минимальный и максимальный коэффициенты масштаба). </summary>
        [Tooltip("Вариация масштаба объектов при спавне (минимальный и максимальный коэффициенты масштаба)."),
         SerializeField, MinMaxSlider(0.5f, 2f)]
        private Vector2 _scaleVariation;

        /// <summary> Список всех заспавненных объектов. </summary>
        protected readonly List<GameObject> _spawnedObjects = new();

        #endregion

        /// <summary> Запускает процесс спавна при старте сцены. </summary>
        private void Start() => SpawnFromConfig();

        /// <summary> Валидация данных. </summary>
        /// <remarks> Коллбэк из UnityAPI. </remarks>
        private void OnValidate()
        {
            if (_config.ObjectPrefab.GetComponent<IHitable>() != null)
                Debug.LogError("В конфиге спавнера не должен находится объект, реализующий интерфейс IHitable." +
                               " Используйте DestroyableContainerSpawner.cs");
        }

        /// <summary> Отображает визуализацию области спавна в редакторе. </summary>
        private void OnDrawGizmos()
        {
            if (!_visualizeConfigArea) return;

            Gizmos.color = _gizmosColor;
            Gizmos.DrawWireCube(transform.position, new Vector3(_config.Width, 1f, _config.Length));
        }

        /// <summary> Спавнит объекты на основе конфигурации. </summary>
        private void SpawnFromConfig()
        {
            if (!_config)
            {
                Debug.LogError("Спавнеру не назначен конфиг.");
                return;
            }

            if (_spawnedObjects.Count != 0) return;

            if (_config.EvenSpread)
                SpawnEvenSpread();
            else
                SpawnRandomly();
        }

        /// <summary> Спавнит объекты равномерно по сетке. </summary>
        // Пояснения по формулам см. здесь: https://www.youtube.com/watch?v=rSKMYc1CQHE&t=122s
        private void SpawnEvenSpread()
        {
            int objectsPerRow = (int)Mathf.Sqrt(_config.Quantity);
            int objectsPerColumn = (_config.Quantity - 1) / objectsPerRow + 1;
            for (int i = 0; i < _config.Quantity; i++)
            {
                float x = (i / (float)objectsPerRow - objectsPerColumn / 2f + 0.5f) * _config.MinSpacing;
                float z = (i % objectsPerRow - objectsPerRow / 2f + 0.5f) * _config.MinSpacing;
                var offset = _config.Width > _config.Length ? new Vector3(x, 0, z) : new Vector3(z, 0, x);
                SpawnObject(transform.position + offset, Random.value * 360f, GetRandomScale());
            }
        }

        /// <summary> Спавнит объекты случайным образом в заданной области. </summary>
        private void SpawnRandomly()
        {
            int iterations = 0;
            while (_spawnedObjects.Count < _config.Quantity && iterations < _maxIterations)
            {
                var position = GetRandomPosition(transform.position);
                if (CanSpawnAtPosition(position)) SpawnObject(position, Random.Range(0, 360f), GetRandomScale());

                iterations++;
            }

            if (_spawnedObjects.Count != _config.Quantity)
                Debug.LogError("Превышен лимит итераций! Увеличьте зону спавна или уменьшите количество объектов");
        }

        /// <summary> Генерирует случайную точку в пределах области спавна. </summary>
        /// <param name="areaCenterPosition"> Центр области спавна. </param>
        /// <returns> Случайная точка в пределах области. </returns>
        private Vector3 GetRandomPosition(Vector3 areaCenterPosition) => new(
            Random.Range(areaCenterPosition.x - _config.Width * 0.5f, areaCenterPosition.x + _config.Width * 0.5f),
            0f,
            Random.Range(areaCenterPosition.z - _config.Length * 0.5f, areaCenterPosition.z + _config.Length * 0.5f)
        );

        /// <summary> Проверяет возможность спавна объекта в заданной позиции. </summary>
        /// <param name="position"> Позиция для проверки. </param>
        /// <returns> Возвращает true, если объект может быть заспавнен в данной позиции, иначе false. </returns>
        private bool CanSpawnAtPosition(Vector3 position) => Physics.OverlapSphere(
            position, _config.MinSpacing, _obstaclesLayerMask, QueryTriggerInteraction.Collide
        ).Length == 0;

        /// <summary> Спавнит объект в заданной позиции с заданными параметрами масштаба и вращения. </summary>
        /// <param name="position"> Позиция спавна объекта. </param>
        /// <param name="rotationY"> Угол вращения объекта по оси Y. </param>
        /// <param name="scale"> Масштаб объекта. </param>
        /// <param name="hitsTaken"> Количственно сделанных ударов по объекту. </param>
        protected virtual GameObject SpawnObject(Vector3 position, float rotationY, Vector3 scale, object data = null)
        {
            var obj = Instantiate(
                _config.ObjectPrefab, position, Quaternion.Euler(0f, rotationY, 0f), transform
            );
            obj.transform.localScale = scale;
            obj.GetComponent<IDestroyable>().OnObjectDestroyed += RemoveObjectFromList;
            _spawnedObjects.Add(obj);
            return obj;
        }

        /// <summary> Удаляет объект из списка заспавненных объектов. </summary>
        /// <param name="destroyable"> Объект, который необходимо удалить из списка. </param>
        private void RemoveObjectFromList(IDestroyable destroyable)
        {
            if (destroyable is MonoBehaviour monoBehaviour)
                _spawnedObjects.Remove(monoBehaviour.gameObject);
        }

        /// <summary> Получает случайный коэффициент масштабирования. </summary>
        /// <returns> Коэффициент масштабирования в виде вектора. </returns>
        private Vector3 GetRandomScale() => Vector3.one * Random.Range(_scaleVariation.x, _scaleVariation.y);

        #region Saving

        /// <summary> Структура для записи состояния заспавненных объектов. </summary>
        [Serializable]
        private struct SpawnedObjectRecord
        {
            public SerializableVector3 Position;
            public float RotationY;
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
    }
}