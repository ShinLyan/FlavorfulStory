using System;
using System.Collections.Generic;
using UnityEngine;
using FlavorfulStory.Saving;
using Random = UnityEngine.Random;
using GD.MinMaxSlider;

public class ObjectSpawner : MonoBehaviour, ISaveable
{
    [SerializeField] private ObjectSpawnerConfig _config;

    [SerializeField] private LayerMask _obstaclesLayerMask;
    
    [SerializeField] private bool _visualizeConfigArea;

    [SerializeField] private Color _gizmosColor;
        
    [SerializeField] private int _maxIterations;

    [MinMaxSlider(0.5f, 2)]
    [SerializeField] private Vector2 _scaleVariation;
    
    private readonly List<GameObject> _spawnedObjects = new();
    
    private List<SpawnedObjectRecord> _spawnedObjectRecords = new(0);

    private bool _loadFromFile = false;
    

    private void Start()
    {
        if (!_loadFromFile) SpawnFromConfig();
    }

    private void SpawnFromConfig()
    {
        if (_config == null) Debug.LogError("Спавнеру не назначен конфиг.");
        
        if (_config.EvenSpread)
        {
            SpawnEvenSpreadFromConfig();
        }
        else
        {
            SpawnRandomlyFromConfig();
        }
    }

    private void SpawnRandomlyFromConfig()
    {
        int objectsSpawned = 0;
        var areaCenter = transform.position;

        int iterations = 0;
        while (objectsSpawned < _config.Quantity && iterations < _maxIterations)
        {
            var spawnPosition = GetRandomPointInArea(areaCenter);
            if (CanSpawnObjectAtPosition(spawnPosition))
            {
                var go = Instantiate(
                    _config.Object, spawnPosition,
                    Quaternion.Euler(0f, Random.value * 360f, 0f),
                    transform
                );
                go.transform.localScale = GetScaleVariation();
                AddObjectToList(go);
                objectsSpawned++;
            }
            iterations++;
        }

        if (objectsSpawned != _config.Quantity)
        {
            Debug.LogError("Превышен лимит итераций спавна! Увеличьте зону спавна или уменьшите количество объектов");
        }
    }

    private void SpawnEvenSpreadFromConfig()
    {
        var objectsPerRow = (int) Mathf.Sqrt(_config.Quantity);
        var objectsPerColumn = (_config.Quantity - 1) / objectsPerRow + 1;

        Vector3 offset;
        
        for (int i = 0; i < _config.Quantity; i++)
        {
            float x = (i / (float) objectsPerRow - objectsPerColumn / 2f + 0.5f) * _config.MinSpacing;
            float z = (i % objectsPerRow - objectsPerRow / 2f + 0.5f) * _config.MinSpacing;

            
            offset = _config.Width > _config.Length ? new Vector3(x, 0, z) : new Vector3(z, 0, x);
            
            var go = Instantiate(
                _config.Object, transform.position + offset,
                Quaternion.Euler(0f, Random.value * 360f, 0f),
                transform
            );
            go.transform.localScale = GetScaleVariation();
            AddObjectToList(go);
        }
    }
    
    private void SpawnFromSave(List<SpawnedObjectRecord> spawnedObjectRecords)
    {
        foreach (var spawnedObjectRecord in spawnedObjectRecords)
        {
            var go = Instantiate(
                _config.Object,
                spawnedObjectRecord.Position.ToVector(),
                Quaternion.Euler(0f, spawnedObjectRecord.RotationY, 0f),
                transform
            );
            go.transform.localScale = new Vector3(spawnedObjectRecord.Scale, spawnedObjectRecord.Scale, spawnedObjectRecord.Scale);
            AddObjectToList(go);
        }
    }

    private Vector3 GetRandomPointInArea(Vector3 areaCenterPosition)
    {
        return new Vector3(
        Random.Range(areaCenterPosition.x - _config.Width * 0.5f, areaCenterPosition.x + _config.Width * 0.5f),
        0f,
        Random.Range(areaCenterPosition.z - _config.Length * 0.5f, areaCenterPosition.z + _config.Length * 0.5f)
        );
    }

    private bool CanSpawnObjectAtPosition(Vector3 position)
    {
        return Physics.OverlapSphere(position, _config.MinSpacing, _obstaclesLayerMask).Length == 0;
    }

    private Vector3 GetScaleVariation()
    {
        var randomValue = Random.Range(_scaleVariation.x, _scaleVariation.y);
        return new Vector3(randomValue, randomValue, randomValue);
    }

    private void RemoveObjectFromList(ISpawnable spawnable)
    {
        var monoBehaviour = spawnable as MonoBehaviour;
        _spawnedObjects.Remove(monoBehaviour?.gameObject);
    }
    
    private void AddObjectToList(GameObject go)
    {
        _spawnedObjects.Add(go);
        var spawnable = go.GetComponent<ISpawnable>();
        spawnable.OnObjectDestroyed += RemoveObjectFromList;
    }
    
    private void OnDrawGizmos()
    {
        if (!_visualizeConfigArea) return;
        
        Gizmos.color = _gizmosColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(_config.Width, 1f, _config.Length));
    }

    #region Saving
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
        print("s");
        var spawnedObjectRecords = new List<SpawnedObjectRecord>();
        foreach (var spawnedObject in _spawnedObjects)
        {
            var spawnedObjectRecord = new SpawnedObjectRecord
            {
                Position = new SerializableVector3(spawnedObject.transform.position),
                RotationY = transform.rotation.y,
                Scale = transform.localScale.x
            };
            spawnedObjectRecords.Add(spawnedObjectRecord);
        }
        return spawnedObjectRecords;
    }

    /// <summary> Восстановление состояния объекта при загрузке. </summary>
    /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
    public void RestoreState(object state)
    {
        print("l");
        _spawnedObjectRecords = state as List<SpawnedObjectRecord>;
        _loadFromFile = _spawnedObjectRecords?.Capacity > 0;
        SpawnFromSave(_spawnedObjectRecords);
    }
    #endregion
}