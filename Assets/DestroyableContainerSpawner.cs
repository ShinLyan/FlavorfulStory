using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.ObjectSpawner;
using FlavorfulStory.Saving;
using FlavorfulStory.SceneManagement;
using UnityEngine;

public class DestroyableContainerSpawner : ObjectSpawner
{
    /// <summary> Список записей заспавненных объектов, используемый для сохранения состояния. </summary>
    private List<SpawnedContainerRecord> _spawnedContainerRecords = new(0);
    
    private void Awake()
    {
        if (_config.ObjectPrefab.GetComponent<IHitable>() == null)
            Debug.LogError("В конфиге спавнера должен находится объект, реализующий интерфейс IHitable");
    }

    private void Start()
    {
        if (!_wasLoadedFromSavefile && !SavingWrapper.SaveFileExists)
            SpawnFromConfig();
    }
    
    [Serializable]
    private struct SpawnedContainerRecord
    {
        public SerializableVector3 Position;
        public float RotationY;
        public float Scale;
        public int HitsTaken;
    }
    
    /// <summary> Фиксация состояния объекта при сохранении. </summary>
    /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
    public override object CaptureState() => _spawnedObjects.Select(spawnedObject => new SpawnedContainerRecord
    {
        Position = new SerializableVector3(spawnedObject.transform.position),
        RotationY = transform.rotation.y,
        Scale = transform.localScale.x,
        HitsTaken = spawnedObject.GetComponent<DestroyableResourceContainer>().HitsTaken
    }).ToList();

    /// <summary> Восстановление состояния объекта при загрузке. </summary>
    /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
    public override void RestoreState(object state)
    {
        print("Aboba");
        print(_wasLoadedFromSavefile);
        if (_wasLoadedFromSavefile) return;

        _spawnedContainerRecords = state as List<SpawnedContainerRecord>;
        _wasLoadedFromSavefile = _spawnedContainerRecords?.Count >= 0;
        SpawnFromSave(_spawnedContainerRecords);
    }

    /// <summary> Восстанавливает заспавненные объекты из сохраненного состояния. </summary>
    /// <param name="spawnedObjectRecords"> Заспавненные объекты. </param>
    private void SpawnFromSave(List<SpawnedContainerRecord> spawnedObjectRecords) =>
        spawnedObjectRecords.ForEach(record =>
            SpawnObject(
                record.Position.ToVector(),
                record.RotationY,
                Vector3.one * record.Scale,
                record.HitsTaken
            )
        );
    
    private void SpawnObject(Vector3 position, float rotationY, Vector3 scale, int hitsTaken = 0)
    {
        var go = Instantiate(_config.ObjectPrefab,
            position,
            Quaternion.Euler(0f, rotationY, 0f),
            transform);
        go.transform.localScale = scale;
        go.GetComponent<IDestroyable>().OnObjectDestroyed += RemoveObjectFromList;
        go.GetComponent<DestroyableResourceContainer>().Initialize();
        //TODO: Добавить сеттер HitsTaken внутри DestroyableResourceContainer
        go.GetComponent<DestroyableResourceContainer>().SetHitCount(hitsTaken);

        _spawnedObjects.Add(go);
    }
}