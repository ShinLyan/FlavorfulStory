using System;
using System.Collections.Generic;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Сохраняет размещённые PlaceableObject на сцене. </summary>
    [RequireComponent(typeof(SaveableEntity))]
    public class PlaceableSceneSaver : MonoBehaviour, ISaveable
    {
        /// <summary> Родительский контейнер для размещаемых объектов. </summary>
        [SerializeField] private Transform _container;

        /// <summary> Контроллер размещения объектов. </summary>
        private PlacementController _placementController;

        /// <summary> Фабрика для создания объектов размещения. </summary>
        private IPrefabFactory<PlaceableObject> _placeableFactory;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="placementController"> Контроллер размещения. </param>
        /// <param name="placeableFactory"> Фабрика для создания объектов. </param>
        [Inject]
        private void Construct(PlacementController placementController,
            IPrefabFactory<PlaceableObject> placeableFactory)
        {
            _placementController = placementController;
            _placeableFactory = placeableFactory;
        }

        /// <summary> Регистрирует объекты сцены при запуске. </summary>
        private void Start() => RegisterScenePlaceables();

        /// <summary> Находит все размещённые объекты и регистрирует их в системе. </summary>
        private void RegisterScenePlaceables()
        {
            foreach (var placeable in GetAllPlaceableObjects())
            {
                placeable.transform.SetParent(_container);
                _placementController.RegisterPlacedObject(placeable.transform.position, placeable);
            }
        }

        /// <summary> Получить все размещённые объекты на сцене. </summary>
        /// <returns> Массив всех размещённых объектов на сцене. </returns>
        private static PlaceableObject[] GetAllPlaceableObjects() =>
            FindObjectsByType<PlaceableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        #region ISaveable

        /// <summary> Структура данных для сериализации размещённых объектов. </summary>
        [Serializable]
        private readonly struct PlaceableObjectSaveRecord
        {
            /// <summary> ID предмета, связанного с объектом. </summary>
            public string ItemID { get; }

            /// <summary> Позиция объекта. </summary>
            public SerializableVector3 Position { get; }

            /// <summary> Поворот объекта. </summary>
            public SerializableVector3 Rotation { get; }

            /// <summary> Состояния компонентов. </summary>
            public Dictionary<string, object> ComponentStates { get; }

            /// <summary> Конструктор структуры сохранения объекта. </summary>
            /// <param name="itemID"> ID предмета. </param>
            /// <param name="position"> Позиция в мире. </param>
            /// <param name="rotation"> Угол поворота. </param>
            /// <param name="componentStates"> Состояния компонентов. </param>
            public PlaceableObjectSaveRecord(string itemID, SerializableVector3 position, SerializableVector3 rotation,
                Dictionary<string, object> componentStates)
            {
                ItemID = itemID;
                Position = position;
                Rotation = rotation;
                ComponentStates = componentStates;
            }
        }

        /// <summary> Сохраняет текущее состояние размещённых объектов. </summary>
        /// <returns> Список данных объектов для сохранения. </returns>
        public object CaptureState()
        {
            var data = new List<PlaceableObjectSaveRecord>();

            foreach (var placeable in GetAllPlaceableObjects())
            {
                var record = new PlaceableObjectSaveRecord(placeable.PlaceableItem.ItemID,
                    new SerializableVector3(placeable.transform.position),
                    new SerializableVector3(placeable.transform.eulerAngles),
                    new Dictionary<string, object>());

                foreach (var saveable in placeable.GetComponents<ISaveable>())
                {
                    string key = saveable.GetType().FullName;
                    if (key != null) record.ComponentStates[key] = saveable.CaptureState();
                }

                data.Add(record);
            }

            return data;
        }

        /// <summary> Восстанавливает состояние объектов из сохранённых данных. </summary>
        /// <param name="state"> Сохранённое состояние. </param>
        public void RestoreState(object state)
        {
            if (state is not List<PlaceableObjectSaveRecord> records) return;

            DestroyAllScenePlaceables();

            foreach (var record in records)
            {
                var placeableItem = ItemDatabase.GetItemFromID(record.ItemID) as PlaceableItem;
                if (!placeableItem) continue;

                var placeable = _placeableFactory.Create(placeableItem.Prefab, parentTransform: _container);
                placeable.transform.SetPositionAndRotation(record.Position.ToVector(),
                    Quaternion.Euler(record.Rotation.ToVector()));

                foreach (var saveable in placeable.GetComponents<ISaveable>())
                {
                    string key = saveable.GetType().FullName;
                    if (key != null && record.ComponentStates.TryGetValue(key, out object savedState))
                        saveable.RestoreState(savedState);
                }

                _placementController.RegisterPlacedObject(placeable.transform.position, placeable);
            }
        }

        /// <summary> Удаляет все размещённые на сцене объекты. </summary>
        private void DestroyAllScenePlaceables()
        {
            foreach (var placeable in GetAllPlaceableObjects())
            {
                _placementController.UnregisterPlacedObject(placeable.transform.position, placeable);
                Destroy(placeable.gameObject);
            }
        }

        #endregion
    }
}