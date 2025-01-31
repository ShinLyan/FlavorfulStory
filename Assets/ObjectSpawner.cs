using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FlavorfulStory.Saving;
using Random = UnityEngine.Random;
using GD.MinMaxSlider;

/// <summary> Класс для управления спавном объектов в сцене. </summary>
public class ObjectSpawner : MonoBehaviour, ISaveable
{
    /// <summary> Конфигурация для спавнера объектов, содержащая параметры для спавна. </summary>
    [SerializeField] private ObjectSpawnerConfig _config;

    /// <summary> Слой, на котором находятся препятствия, с которыми не должны пересекаться объекты. </summary>
    [SerializeField] private LayerMask _obstaclesLayerMask;
    
    /// <summary> Если true, будет отображаться область конфигурации для спавна в редакторе. </summary>
    [SerializeField] private bool _visualizeConfigArea;

    /// <summary> Цвет для визуализации области спавна в редакторе. </summary>
    [SerializeField] private Color _gizmosColor;
    
    /// <summary> Максимальное количество попыток спавна объектов. </summary>
    [SerializeField] private int _maxIterations;

    /// <summary> Вариация масштаба объектов при спавне (минимальный и максимальный коэффициенты масштаба). </summary>
    [MinMaxSlider(0.5f, 2)]
    [SerializeField] private Vector2 _scaleVariation;
    
    /// <summary> Список всех заспавненных объектов. </summary>
    private readonly List<GameObject> _spawnedObjects = new();
    
    /// <summary> Список записей заспавненных объектов, используемый для сохранения состояния. </summary>
    private List<SpawnedObjectRecord> _spawnedObjectRecords = new(0);

    /// <summary> Флаг, указывающий, были ли объекты загружены из файла. </summary>
    private bool _loadFromFile;
    
    /// <summary> Запускает процесс спавна при старте сцены. </summary>
    private void Start()
    {
        if (!_loadFromFile) SpawnFromConfig();
    }
    
    /// <summary> Спавнит объекты на основе конфигурации. </summary>
    private void SpawnFromConfig()
    {
        if (_config == null) Debug.LogError("Спавнеру не назначен конфиг.");
        
        if (_config.EvenSpread)
            SpawnEvenSpreadFromConfig();
        else
            SpawnRandomlyFromConfig();
    }

    /// <summary> Спавнит объекты случайным образом в заданной области. </summary>
    private void SpawnRandomlyFromConfig()
    {
        int iterations = 0;
        var areaCenter = transform.position;
        
        while (_spawnedObjects.Count < _config.Quantity && iterations < _maxIterations)
        {
            var spawnPosition = GetRandomPointInArea(areaCenter);
            if (CanSpawnObjectAtPosition(spawnPosition))
            {
                SpawnObject(spawnPosition, Random.value * 360f, GetScaleVariation());
            }
            iterations++;
        }

        if (_spawnedObjects.Count != _config.Quantity)
        {
            Debug.LogError("Превышен лимит итераций спавна! Увеличьте зону спавна или уменьшите количество объектов");
        }
    }

    /// <summary> Спавнит объекты равномерно по сетке. </summary>
    private void SpawnEvenSpreadFromConfig()
    {
        var objectsPerRow = (int) Mathf.Sqrt(_config.Quantity);
        var objectsPerColumn = (_config.Quantity - 1) / objectsPerRow + 1;
        
        for (int i = 0; i < _config.Quantity; i++)
        {
            float x = (i / (float) objectsPerRow - objectsPerColumn / 2f + 0.5f) * _config.MinSpacing;
            float z = (i % objectsPerRow - objectsPerRow / 2f + 0.5f) * _config.MinSpacing;
            var offset = _config.Width > _config.Length ? new Vector3(x, 0, z) : new Vector3(z, 0, x);
            
            SpawnObject(transform.position + offset, Random.value * 360f, GetScaleVariation());
        }
    }
    
    /// <summary> Восстанавливает заспавненные объекты из сохраненного состояния. </summary>
    private void SpawnFromSave(List<SpawnedObjectRecord> spawnedObjectRecords)
    {
        foreach (var spawnedObjectRecord in spawnedObjectRecords)
        {
            SpawnObject(
                spawnedObjectRecord.Position.ToVector(), spawnedObjectRecord.RotationY,
                new Vector3(spawnedObjectRecord.Scale, spawnedObjectRecord.Scale, spawnedObjectRecord.Scale)
            );
        }
    }

    /// <summary> Генерирует случайную точку в пределах области спавна. </summary>
    /// <param name="areaCenterPosition"> Центр области спавна. </param>
    /// <returns> Случайная точка в пределах области. </returns>
    private Vector3 GetRandomPointInArea(Vector3 areaCenterPosition)
    {
        return new Vector3(
            Random.Range(areaCenterPosition.x - _config.Width * 0.5f, areaCenterPosition.x + _config.Width * 0.5f),
            0f,
            Random.Range(areaCenterPosition.z - _config.Length * 0.5f, areaCenterPosition.z + _config.Length * 0.5f)
        );
    }

    /// <summary> Проверяет возможность спавна объекта в заданной позиции. </summary>
    /// <param name="position"> Позиция для проверки. </param>
    /// <returns> Возвращает true, если объект может быть заспавнен в данной позиции, иначе false. </returns>
    private bool CanSpawnObjectAtPosition(Vector3 position)
    {
        return Physics.OverlapSphere(position, _config.MinSpacing, _obstaclesLayerMask).Length == 0;
    }

    /// <summary> Получает случайный коэффициент масштабирования. </summary>
    /// <returns> Коэффициент масштабирования в виде вектора. </returns>
    private Vector3 GetScaleVariation()
    {
        var randomValue = Random.Range(_scaleVariation.x, _scaleVariation.y);
        return new Vector3(randomValue, randomValue, randomValue);
    }

    /// <summary> Удаляет объект из списка заспавненных объектов. </summary>
    /// <param name="destroyable"> Объект, который необходимо удалить из списка. </param>
    private void RemoveObjectFromList(IDestroyable destroyable)
    {
        var monoBehaviour = destroyable as MonoBehaviour;
        _spawnedObjects.Remove(monoBehaviour?.gameObject);
    }
    
    /// <summary> Добавляет объект в список заспавненных объектов и подписывается на событие его уничтожения. </summary>
    /// <param name="go"> Игровой объект, который нужно добавить в список. </param>
    private void AddObjectToList(GameObject go)
    {
        _spawnedObjects.Add(go);
        go.GetComponent<IDestroyable>().OnObjectDestroyed += RemoveObjectFromList;
    }

    /// <summary> Спавнит объект в заданной позиции с заданными параметрами масштаба и вращения. </summary>
    /// <param name="position"> Позиция спавна объекта. </param>
    /// <param name="rotationY"> Угол вращения объекта по оси Y. </param>
    /// <param name="scale"> Масштаб объекта. </param>
    private void SpawnObject(Vector3 position, float rotationY, Vector3 scale)
    {
        var go = Instantiate(_config.Object, position, Quaternion.Euler(0f, rotationY, 0f), transform);
        go.transform.localScale = scale;
        AddObjectToList(go);
    }
    
    /// <summary> Отображает визуализацию области спавна в редакторе. </summary>
    private void OnDrawGizmos()
    {
        if (!_visualizeConfigArea) return;
        
        Gizmos.color = _gizmosColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(_config.Width, 1f, _config.Length));
    }

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
    public object CaptureState()
    {
        return _spawnedObjects.Select(spawnedObject => new SpawnedObjectRecord
        {
            Position = new SerializableVector3(spawnedObject.transform.position),
            RotationY = transform.rotation.y,
            Scale = transform.localScale.x
        }).ToList();
    }

    /// <summary> Восстановление состояния объекта при загрузке. </summary>
    /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
    public void RestoreState(object state)
    {
        _spawnedObjectRecords = state as List<SpawnedObjectRecord>;
        _loadFromFile = _spawnedObjectRecords?.Capacity > 0;
        SpawnFromSave(_spawnedObjectRecords);
    }
    #endregion
}