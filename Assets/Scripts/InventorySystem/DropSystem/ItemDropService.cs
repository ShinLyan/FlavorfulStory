using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InventorySystem.PickupSystem;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.DropSystem
{
    /// <summary> Сервис, отвечающий за выброс предметов из инвентаря в игровом мире. </summary>
    public class ItemDropService : IItemDropService, ISaveable
    {
        /// <summary> Стандартная задержка перед возможностью подбора предмета. </summary>
        private const float DefaultPickupDelay = 1.5f;

        /// <summary> Контейнер, в котором спавнятся все выброшенные предметы. </summary>
        private Transform _container;

        /// <summary> Фабрика создания объектов Pickup'ов. </summary>
        private readonly PickupFactory _pickupFactory;

        /// <summary> Список заспавненных Pickup для сохранения и очистки. </summary>
        private readonly List<Pickup> _spawnedPickups = new();

        /// <summary> Конструктор сервиса выброса предметов. </summary>
        /// <param name="pickupFactory"> Фабрика создания Pickup объектов. </param>
        public ItemDropService(PickupFactory pickupFactory) => _pickupFactory = pickupFactory;

        /// <summary> Спавнит предмет в мире с заданным количеством и задержкой. </summary>
        private Pickup Spawn(InventoryItem item, int quantity, Vector3 position, float pickupDelay = 1f)
        {
            if (!_container) Debug.LogError($"{nameof(ItemDropService)}.{nameof(Spawn)}: Container is not set)");

            var pickup = _pickupFactory.Create(item, position, quantity, pickupDelay, _container);
            if (pickup) _spawnedPickups.Add(pickup);
            return pickup;
        }

        /// <summary> Применяет силу к Rigidbody предмета, если она задана. </summary>
        private static void ApplyForce(Pickup pickup, Vector3 force)
        {
            if (pickup.TryGetComponent(out Rigidbody rb)) rb.AddForce(force, ForceMode.Impulse);
        }

        #region Drop Methods

        /// <inheritdoc/>
        public void Drop(InventoryItem item, int quantity,
            Vector3 position, float pickupDelay = DefaultPickupDelay)
        {
            Spawn(item, quantity, position);
        }

        /// <inheritdoc/>
        public void Drop(InventoryItem item, int quantity, Vector3 position,
            Vector3 force, float pickupDelay = DefaultPickupDelay)
        {
            var pickup = Spawn(item, quantity, position);
            if (pickup) ApplyForce(pickup, force);
        }

        /// <inheritdoc/>
        public void DropFromInventory(Inventory inventory, int slotIndex, Vector3 position,
            float pickupDelay = DefaultPickupDelay)
        {
            if (!TryConsumeInventorySlot(inventory, slotIndex, out var item, out int quantity)) return;
            Drop(item, quantity, position, pickupDelay);
        }

        /// <inheritdoc/>
        public void DropFromInventory(Inventory inventory, int slotIndex, Vector3 position,
            Vector3 force, float pickupDelay = DefaultPickupDelay)
        {
            if (!TryConsumeInventorySlot(inventory, slotIndex, out var item, out int quantity)) return;
            Drop(item, quantity, position, force, pickupDelay);
        }

        /// <summary> Извлекает предмет и его количество из слота инвентаря и удаляет его. </summary>
        private static bool TryConsumeInventorySlot(Inventory inventory, int slotIndex, out InventoryItem item,
            out int quantity)
        {
            item = inventory.GetItemInSlot(slotIndex);
            quantity = inventory.GetNumberInSlot(slotIndex);

            if (quantity <= 0 || item == null)
            {
                Debug.LogError($"[ItemDropService] Slot {slotIndex} is empty.");
                return false;
            }

            inventory.RemoveFromSlot(slotIndex);
            return true;
        }

        /// <inheritdoc/>
        public void SetDroppedItemsContainer(Transform container) => _container = container;

        #endregion

        #region Saving

        /// <summary> Структура для сериализации информации о выброшенных предметах. </summary>
        [Serializable]
        private struct DropSaveData
        {
            public string ItemID;
            public SerializableVector3 Position;
            public int Quantity;
        }

        /// <summary> Сохраняет состояние всех выброшенных предметов. </summary>
        public object CaptureState()
        {
            _spawnedPickups.RemoveAll(p => p == null);
            return _spawnedPickups.Select(p => new DropSaveData
            {
                ItemID = p.Item.ItemID,
                Position = new SerializableVector3(p.transform.position),
                Quantity = p.Number
            }).ToList();
        }

        /// <summary> Восстанавливает выброшенные предметы из сохранения. </summary>
        public void RestoreState(object state)
        {
            if (state is not List<DropSaveData> records) return;

            foreach (var record in records)
            {
                var item = ItemDatabase.GetItemFromID(record.ItemID);
                if (item != null) Spawn(item, record.Quantity, record.Position.ToVector());
            }
        }

        #endregion
    }
}