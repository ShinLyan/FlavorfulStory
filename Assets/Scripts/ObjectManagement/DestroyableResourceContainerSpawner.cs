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
        /// <param name="data"> Дополнительные данные. </param>
        protected override GameObject SpawnObject(Vector3 position, float rotationY, Vector3 scale, object data = null)
        {
            var obj = base.SpawnObject(position, rotationY, scale);
            if (data == null) return obj;

            int hitsTaken = (int)data;
            obj.GetComponent<DestroyableResourceContainer>().Initialize(hitsTaken);
            return obj;
        }

        #region ISaveable

        /// <summary> Запись добываемого объекта. </summary>
        [Serializable]
        private readonly struct SpawnedContainerRecord
        {
            /// <summary> Структура для записи состояния заспавненных объектов. </summary>
            public SpawnedObjectRecord ObjectRecord { get; }

            /// <summary> Количество полученных ударов. </summary>
            public int HitsTaken { get; }

            /// <summary> Конструктор с параметрами. </summary>
            /// <param name="objectRecord"> Структура для записи состояния заспавненных объектов. </param>
            /// <param name="hitsTaken"> Количество полученных ударов. </param>
            public SpawnedContainerRecord(SpawnedObjectRecord objectRecord, int hitsTaken)
            {
                ObjectRecord = objectRecord;
                HitsTaken = hitsTaken;
            }
        }

        /// <summary> Фиксация состояния объекта при сохранении. </summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
        public override object CaptureState() => _spawnedObjects.Select(spawnedObject => new SpawnedContainerRecord(
            new SpawnedObjectRecord(new SerializableVector3(spawnedObject.transform.position),
                spawnedObject.transform.eulerAngles.y, spawnedObject.transform.localScale.x),
            spawnedObject.GetComponent<DestroyableResourceContainer>().HitsTaken)).ToList();

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public override void RestoreState(object state)
        {
            if (state is not List<SpawnedContainerRecord> records) return;

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