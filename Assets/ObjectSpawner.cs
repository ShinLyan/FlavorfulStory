using System.Collections.Generic;
using FlavorfulStory.Saving;
using GD.MinMaxSlider;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectSpawner : MonoBehaviour, ISaveable
{
    [SerializeField] private ObjectSpawnerConfig _config;

    [SerializeField] private LayerMask _obstaclesLayerMask;
    
    [SerializeField] private bool _visualizeConfigArea;
        
    [SerializeField] private int MaxIterations = 300;

    [MinMaxSlider(0.5f, 2)]
    [SerializeField] private Vector2 _scaleVariation;
    
    private List<GameObject> _spawnedObjects;

    private void Awake()
    {
        _spawnedObjects = new List<GameObject>(_config.Quantity);
    }

    private void Start()
    {
        SpawnFromConfig();
    }

    private void SpawnFromConfig()
    {
        if (_config == null) Debug.LogError("Спавнеру не назначен конфиг.");

        int objectsSpawned = 0;
        var areaCenter = transform.position;

        int iterations = 0;
        while (objectsSpawned < _config.Quantity && iterations < MaxIterations)
        {
            var spawnPosition = GetRandomPointInArea(areaCenter);
            if (CanSpawnObjectAtPosition(spawnPosition))
            {
                var go = Instantiate(_config.Object, spawnPosition, Quaternion.Euler(0f, Random.value * 360f, 0f));
                go.transform.localScale = GetScaleVariation();
                objectsSpawned++;
            }
            iterations++;
        }

        if (objectsSpawned != _config.Quantity)
        {
            Debug.LogError("Превышен лимит итераций спавна! Увеличьте зону спавна или уменьшите количество объектов");
        }
    }

    private void SpawnFromSave(List<Vector3> positions)
    {
        
    }

    private Vector3 GetRandomPointInArea(Vector3 areaCenterPosition)
    {
        return new Vector3(
        Random.Range(areaCenterPosition.x - _config.Width * 0.5f, areaCenterPosition.x + _config.Width * 0.5f),
        0f,
        Random.Range(areaCenterPosition.x - _config.Length * 0.5f, areaCenterPosition.x + _config.Length * 0.5f)
        );
    }

    private bool CanSpawnObjectAtPosition(Vector3 position)
    {
        return Physics.OverlapSphere(position, _config.MinSpacing, _obstaclesLayerMask).Length == 0;
    }

    private Vector3 GetScaleVariation()
    {
        return new Vector3(
            Random.Range(_scaleVariation.x, _scaleVariation.y),
            Random.Range(_scaleVariation.x, _scaleVariation.y),
            Random.Range(_scaleVariation.x, _scaleVariation.y)
        );
    }
    
    public object CaptureState()
    {
        return null;
    }

    public void RestoreState(object state)
    {
        
    }
}
