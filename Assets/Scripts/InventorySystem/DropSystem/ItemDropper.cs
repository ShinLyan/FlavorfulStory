using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InventorySystem.PickupSystem;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace FlavorfulStory.InventorySystem.DropSystem
{
    /// <summary> Выбрасыватель предметов. </summary>
    /// <remarks> Может быть размещен на объекте сцены, которому необходимо выбрасывать pickup в мир.
    /// Также отслеживается системой сохранения. </remarks>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        /// <summary> Выброшенные предметы. </summary>
        private List<Pickup> _droppedItems = new();

        /// <summary> Фабрика для создания экземпляров предметов, доступных для подбора. </summary>
        private PickupFactory _pickupFactory;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="pickupFactory"> Фабрика для создания экземпляров предметов, доступных для подбора. </param>
        [Inject]
        private void Construct(PickupFactory pickupFactory) => _pickupFactory = pickupFactory;

        /// <summary> Создание pickup в определенной позиции. </summary>
        /// <param name="item"> Предмет, который необходимо заспавнить. </param>
        /// <param name="number"> Количество предметов. </param>
        public void DropItem(InventoryItem item, int number) => SpawnPickup(item, number, GetDropPosition());

        /// <summary> Получить позицию для спавна предмета. </summary>
        /// <remarks> Можете переопределить для задания кастомной позиции спавна предмета. </remarks>
        /// <returns> Возвращает позицию, где должен быть заспавнен предмет. </returns>
        private Vector3 GetDropPosition()
        {
            const float DropOffsetRange = 2f; // Диапазон случайного смещения по осям X и Z
            float offsetX = Random.Range(-DropOffsetRange, DropOffsetRange);
            float offsetZ = Random.Range(-DropOffsetRange, DropOffsetRange);
            return transform.position + new Vector3(offsetX, 1, offsetZ);
        }

        /// <summary> Заспавнить предмет Pickup на сцене. </summary>
        /// <param name="item"> Предмет, который необходимо заспавнить. </param>
        /// <param name="number"> Количество предметов. </param>
        /// <param name="spawnPosition"> Позиция спавна предмета. </param>
        private void SpawnPickup(InventoryItem item, int number, Vector3 spawnPosition)
        {
            var pickup = _pickupFactory.Create(item, spawnPosition, number);
            _droppedItems.Add(pickup);
        }

        #region Saving

        /// <summary> Запись о выпавших предметах. </summary>
        [Serializable]
        private struct DropRecord
        {
            /// <summary> ID выпавшего предмета. </summary>
            public string ItemID;

            /// <summary> Позиция выпадшего предмета. </summary>
            public SerializableVector3 Position;

            /// <summary> Количество выпавших предметов. </summary>
            public int Quantity;
        }

        /// <summary> Фиксация состояния объекта при сохранении. </summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
        public object CaptureState()
        {
            RemovePickedUpItems();

            var droppedItemsList = _droppedItems.Select(pickup => new DropRecord
            {
                ItemID = pickup.Item.ItemID,
                Position = new SerializableVector3(pickup.transform.position),
                Quantity = pickup.Number
            }).ToList();

            return droppedItemsList;
        }

        /// <summary> Удалить из списка все предметы, которые были подобраны в мире. </summary>
        private void RemovePickedUpItems() => _droppedItems = _droppedItems.Where(item => item).ToList();

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public void RestoreState(object state)
        {
            if (state is not List<DropRecord> dropRecords) return;

            foreach (var dropRecord in dropRecords)
            {
                var pickupItem = ItemDatabase.GetItemFromID(dropRecord.ItemID);
                int number = dropRecord.Quantity;
                var position = dropRecord.Position.ToVector();
                SpawnPickup(pickupItem, number, position);
            }
        }

        #endregion
    }
}