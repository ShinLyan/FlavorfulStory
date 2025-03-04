using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.ResourceContainer;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.ObjectManagement
{
    /// <summary> Спавнер добываемых объектов. </summary>
    /// <remarks> Наследник от <see cref="ObjectSpawner" />. </remarks>
    public class DestroyableResourceContainerSpawner : ObjectSpawner
    {
        /// <summary> Список записей заспавненных объектов, используемый для сохранения состояния. </summary>
        private List<SpawnedContainerRecord> _spawnedContainerRecords = new();

        /// <summary> Валидация данных. </summary>
        /// <remarks> Коллбэк из UnityAPI. </remarks>
        private void OnValidate()
        {
            if (_config.ObjectPrefab.GetComponent<IHitable>() == null)
                Debug.LogError("В конфиге спавнера должен находится объект, реализующий интерфейс IHitable");
        }

        /// <summary> Заспавнить добываемый объект. </summary>
        /// <param name="position"> Глобальная позиция в мировых координатах. </param>
        /// <param name="rotationY"> Угол поворота вокруг оси Y. </param>
        /// <param name="scale"> Масштаб. </param>
        /// <param name="hitsTaken"> Количество нанесенных ударов. </param>
        protected override GameObject SpawnObject(Vector3 position, float rotationY, Vector3 scale, object data = null)
        {
            var obj = base.SpawnObject(position, rotationY, scale);
            if (data == null) return obj;

            int hitsTaken = (int)data;
            obj.GetComponent<DestroyableResourceContainer>().Initialize(hitsTaken);
            return obj;
        }

        #region Saving

        /// <summary> Запись добываемого объекта. </summary>
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
            RotationY = spawnedObject.transform.eulerAngles.y,
            Scale = spawnedObject.transform.localScale.x,
            HitsTaken = spawnedObject.GetComponent<DestroyableResourceContainer>().HitsTaken
        }).ToList();

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public override void RestoreState(object state)
        {
            if (_spawnedObjects != null && _spawnedObjects.Count != 0) DestroySpawnedObjects();

            _spawnedContainerRecords = state as List<SpawnedContainerRecord>;
            SpawnFromSave(_spawnedContainerRecords);
        }

        /// <summary> Восстанавливает заспавненные объекты из сохраненного состояния. </summary>
        /// <param name="records"> Заспавненные объекты. </param>
        private void SpawnFromSave(List<SpawnedContainerRecord> records)
        {
            foreach (var record in records)
                SpawnObject(record.Position.ToVector(), record.RotationY, Vector3.one * record.Scale, record.HitsTaken);
        }

        #endregion
    }
}