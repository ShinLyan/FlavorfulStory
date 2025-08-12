using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.GridSystem;
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

        /// <summary> Провайдер координат грида. </summary>
        private GridPositionProvider _gridPositionProvider;

        /// <summary> Контроллер размещения объектов. </summary>
        private PlacementController _placementController;

        /// <summary> Фабрика для создания объектов размещения. </summary>
        private IPrefabFactory<PlaceableObject> _placeableFactory;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="gridPositionProvider"> Провайдер координат грида. </param>
        /// <param name="placementController"> Контроллер размещения. </param>
        /// <param name="placeableFactory"> Фабрика для создания объектов. </param>
        [Inject]
        private void Construct(GridPositionProvider gridPositionProvider, PlacementController placementController,
            IPrefabFactory<PlaceableObject> placeableFactory)
        {
            _gridPositionProvider = gridPositionProvider;
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
                var gridPosition = _gridPositionProvider.WorldToGrid(placeable.transform.position);
                _placementController.RegisterPlacedObject(gridPosition, placeable);
            }
        }

        /// <summary> Получить все размещённые объекты на сцене. </summary>
        /// <returns> Массив всех размещённых объектов на сцене. </returns>
        private static PlaceableObject[] GetAllPlaceableObjects() =>
            FindObjectsByType<PlaceableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        #region ISaveable

        /// <summary> Структура данных для сериализации размещённых объектов. </summary>
        [Serializable]
        private readonly struct PlaceableSaveData
        {
            /// <summary> ID предмета, связанного с объектом. </summary>
            public string ItemID { get; }

            /// <summary> Позиция объекта. </summary>
            public SerializableVector3 Position { get; }

            /// <summary> Поворот объекта. </summary>
            public SerializableVector3 Rotation { get; }

            /// <summary> Конструктор структуры сохранения объекта. </summary>
            /// <param name="itemID"> ID предмета. </param>
            /// <param name="position"> Позиция в мире. </param>
            /// <param name="rotation"> Угол поворота. </param>
            public PlaceableSaveData(string itemID, SerializableVector3 position, SerializableVector3 rotation)
            {
                ItemID = itemID;
                Position = position;
                Rotation = rotation;
            }
        }

        /// <summary> Сохраняет текущее состояние размещённых объектов. </summary>
        /// <returns> Список данных объектов для сохранения. </returns>
        public object CaptureState() => GetAllPlaceableObjects().Select(placeable =>
            new PlaceableSaveData(placeable.PlaceableItem.ItemID, new SerializableVector3(placeable.transform.position),
                new SerializableVector3(placeable.transform.eulerAngles))).ToList();

        /// <summary> Восстанавливает состояние объектов из сохранённых данных. </summary>
        /// <param name="state"> Сохранённое состояние. </param>
        public void RestoreState(object state)
        {
            if (state is not List<PlaceableSaveData> savedList) return;

            DestroyAllScenePlaceables();

            foreach (var record in savedList)
            {
                var placeableItem = ItemDatabase.GetItemFromID(record.ItemID) as PlaceableItem;
                if (!placeableItem) continue;

                var placeable = _placeableFactory.Create(placeableItem.Prefab, _container);
                placeable.transform.SetPositionAndRotation(record.Position.ToVector(),
                    Quaternion.Euler(record.Rotation.ToVector()));

                if (!placeable) continue;

                var gridPosition = _gridPositionProvider.WorldToGrid(placeable.transform.position);
                _placementController.RegisterPlacedObject(gridPosition, placeable);
            }
        }

        /// <summary> Удаляет все размещённые на сцене объекты. </summary>
        private void DestroyAllScenePlaceables()
        {
            foreach (var placeable in GetAllPlaceableObjects())
            {
                var gridPosition = _gridPositionProvider.WorldToGrid(placeable.transform.position);
                _placementController.UnregisterPlacedObject(gridPosition, placeable);

                Destroy(placeable.gameObject);
            }
        }

        #endregion
    }
}