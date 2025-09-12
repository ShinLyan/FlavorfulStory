using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Utils.Factories;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Сервис сохранения и восстановления размещённых объектов. </summary>
    public class PlaceableSaveService : ISaveableService, IInitializable
    {
        /// <summary> Провайдер размещаемых объектов. </summary>
        private readonly IPlaceableObjectProvider _provider;

        /// <summary> Контроллер размещения объектов. </summary>
        private readonly PlacementController _controller;

        /// <summary> Фабрика для создания объектов размещения. </summary>
        private readonly IPrefabFactory<PlaceableObject> _factory;

        /// <summary> Родительский контейнер для размещаемых объектов. </summary>
        private readonly Transform _container;

        /// <summary> Конструктор с параметрами. </summary>
        /// <param name="provider"> Провайдер размещаемых объектов. </param>
        /// <param name="controller"> Контроллер размещения объектов. </param>
        /// <param name="factory"> Фабрика для создания объектов размещения. </param>
        /// <param name="container"> Родительский контейнер для размещаемых объектов. </param>
        public PlaceableSaveService(IPlaceableObjectProvider provider, PlacementController controller,
            IPrefabFactory<PlaceableObject> factory, Transform container)
        {
            _provider = provider;
            _controller = controller;
            _factory = factory;
            _container = container;
        }

        /// <summary> Инициализация сервиса. </summary>
        public void Initialize() => RegisterPlaceables();

        /// <summary> Регистрирует все уже размещённые объекты при старте сцены. </summary>
        private void RegisterPlaceables()
        {
            foreach (var placeable in _provider.All)
            {
                placeable.transform.SetParent(_container);
                _controller.RegisterPlacedObject(placeable.transform.position, placeable);
            }
        }

        #region ISaveable

        /// <summary> Структура данных для сериализации размещённых объектов. </summary>
        [Serializable]
        private readonly struct PlaceableRecord
        {
            /// <summary> ID предмета, связанного с объектом. </summary>
            public string ItemId { get; }

            /// <summary> Позиция объекта. </summary>
            public SerializableVector3 Position { get; }

            /// <summary> Поворот объекта. </summary>
            public SerializableVector3 Rotation { get; }

            /// <summary> Состояния компонентов. </summary>
            public Dictionary<string, object> ComponentStates { get; }

            /// <summary> Конструктор структуры сохранения объекта. </summary>
            /// <param name="itemId"> ID предмета. </param>
            /// <param name="position"> Позиция в мире. </param>
            /// <param name="rotation"> Угол поворота. </param>
            /// <param name="componentStates"> Состояния компонентов. </param>
            public PlaceableRecord(string itemId, SerializableVector3 position, SerializableVector3 rotation,
                Dictionary<string, object> componentStates)
            {
                ItemId = itemId;
                Position = position;
                Rotation = rotation;
                ComponentStates = componentStates;
            }
        }

        /// <summary> Сохраняет текущее состояние размещённых объектов. </summary>
        /// <returns> Список данных объектов для сохранения. </returns>
        public object CaptureState()
        {
            var records = new List<PlaceableRecord>();

            foreach (var placeable in _provider.All)
            {
                var componentStates = new Dictionary<string, object>();
                foreach (var saveable in placeable.GetComponents<ISaveable>())
                {
                    string key = saveable.GetType().FullName;
                    if (key != null) componentStates[key] = saveable.CaptureState();
                }

                records.Add(new PlaceableRecord(placeable.PlaceableItem.ItemID,
                    new SerializableVector3(placeable.transform.position),
                    new SerializableVector3(placeable.transform.eulerAngles),
                    componentStates));
            }

            return records;
        }

        /// <summary> Восстанавливает состояние объектов из сохранённых данных. </summary>
        /// <param name="state"> Сохранённое состояние. </param>
        public void RestoreState(object state)
        {
            if (state is not List<PlaceableRecord> records) return;

            DestroyAllPlaceables();

            foreach (var record in records)
            {
                var placeableItem = ItemDatabase.GetItemFromID(record.ItemId) as PlaceableItem;
                if (!placeableItem) continue;

                var placeable = _factory.Create(placeableItem.Prefab, parentTransform: _container);
                placeable.transform.SetPositionAndRotation(record.Position.ToVector(),
                    Quaternion.Euler(record.Rotation.ToVector()));

                foreach (var saveable in placeable.GetComponents<ISaveable>())
                {
                    string key = saveable.GetType().FullName;
                    if (key != null && record.ComponentStates.TryGetValue(key, out object savedState))
                        saveable.RestoreState(savedState);
                }

                _controller.RegisterPlacedObject(placeable.transform.position, placeable);
            }
        }

        /// <summary> Удаляет все размещённые на сцене объекты. </summary>
        private void DestroyAllPlaceables()
        {
            var placeables = _provider.All.ToList();
            foreach (var placeable in placeables)
            {
                _controller.UnregisterPlacedObject(placeable.transform.position, placeable);
                Object.Destroy(placeable.gameObject);
            }
        }

        #endregion
    }
}