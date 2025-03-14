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
            /// <summary> Структура для записи состояния заспавненных объектов. </summary>
            public SpawnedObjectRecord ObjectRecord;

            /// <summary> Количество полученных ударов. </summary>
            public int HitsTaken;
        }

        /// <summary> Фиксация состояния объекта при сохранении. </summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
        public override object CaptureState() => _spawnedObjects.Select(spawnedObject => new SpawnedContainerRecord
        {
            ObjectRecord = new SpawnedObjectRecord
            {
                Position = new SerializableVector3(spawnedObject.transform.position),
                RotationY = spawnedObject.transform.eulerAngles.y,
                Scale = spawnedObject.transform.localScale.x
            },
            HitsTaken = spawnedObject.GetComponent<DestroyableResourceContainer>().HitsTaken
        }).ToList();

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public override void RestoreState(object state)
        {
            if (_spawnedObjects != null && _spawnedObjects.Count != 0) DestroySpawnedObjects();

            var spawnedContainerRecords = state as List<SpawnedContainerRecord>;
            SpawnFromSave(spawnedContainerRecords);
        }

        /// <summary> Восстанавливает заспавненные объекты из сохраненного состояния. </summary>
        /// <param name="records"> Заспавненные объекты. </param>
        private void SpawnFromSave(List<SpawnedContainerRecord> records)
        {
            foreach (var record in records)
                SpawnObject(record.ObjectRecord.Position.ToVector(), record.ObjectRecord.RotationY,
                    Vector3.one * record.ObjectRecord.Scale, record.HitsTaken);
        }

        #endregion

        #region Debug

        /// <summary> Валидация данных. </summary>
        /// <remarks> Коллбэк из UnityAPI. </remarks>
        private void OnValidate()
        {
            if (_spawnObjectPrefab.GetComponent<IHitable>() == null)
                Debug.LogError("В конфиге спавнера должен находится объект, реализующий интерфейс IHitable");
        }

        #endregion
    }
}