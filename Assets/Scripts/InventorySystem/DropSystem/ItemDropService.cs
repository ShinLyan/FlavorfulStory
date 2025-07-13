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
        /// <summary> Задержка перед возможностью подбора предмета. </summary>
        private const float PickupDelay = 1.5f;

        /// <summary> Контейнер, в котором спавнятся все выброшенные предметы. </summary>
        private Transform _container;

        /// <summary> Фабрика создания объектов Pickup'ов. </summary>
        private readonly PickupFactory _pickupFactory;

        /// <summary> Список заспавненных Pickup для сохранения и очистки. </summary>
        private readonly List<Pickup> _spawnedPickups = new();

        /// <summary> Конструктор сервиса выброса предметов. </summary>
        /// <param name="pickupFactory"> Фабрика создания Pickup объектов. </param>
        public ItemDropService(PickupFactory pickupFactory) => _pickupFactory = pickupFactory;

        /// <summary> Выбрасывает предмет в мир в указанной позиции с опциональной силой. </summary>
        /// <param name="item"> Предмет для выбрасывания. </param>
        /// <param name="quantity"> Количество выбрасываемого предмета. </param>
        /// <param name="position"> Позиция появления предмета. </param>
        /// <param name="force"> Применяемая сила (например, для отталкивания). </param>
        public void Drop(InventoryItem item, int quantity, Vector3 position, Vector3? force = null)
        {
            var pickup = Spawn(item, quantity, position, PickupDelay);
            if (pickup && force.HasValue) ApplyForce(pickup, force.Value);
        }

        /// <summary> Выбрасывает предмет из конкретного слота инвентаря. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="slotIndex"> Индекс слота, из которого берется предмет. </param>
        /// <param name="position"> Позиция появления предмета. </param>
        /// <param name="force"> Применяемая сила (необязательно). </param>
        public void DropFromInventory(Inventory inventory, int slotIndex, Vector3 position, Vector3? force = null)
        {
            if (!TryConsumeInventorySlot(inventory, slotIndex, out var item, out int quantity)) return;
            Drop(item, quantity, position, force);
        }

        /// <summary> Устанавливает контейнер, в котором будут размещаться выброшенные предметы. </summary>
        /// <param name="container"> Объект-контейнер на сцене. </param>
        public void SetDroppedItemsContainer(Transform container) => _container = container;

        /// <summary> Спавнит предмет в мире с заданным количеством и задержкой. </summary>
        /// <param name="item"> Предмет для спавна. </param>
        /// <param name="quantity"> Количество предметов. </param>
        /// <param name="position"> Позиция появления. </param>
        /// <param name="pickupDelay"> Задержка перед возможностью поднятия. </param>
        /// <returns> Ссылка на созданный Pickup. </returns>
        private Pickup Spawn(InventoryItem item, int quantity, Vector3 position, float pickupDelay = 1f)
        {
            if (!_container) Debug.LogError($"{nameof(ItemDropService)}.{nameof(Spawn)}: Container is not set)");

            var pickup = _pickupFactory.Create(item, position, quantity, pickupDelay, _container);
            if (pickup) _spawnedPickups.Add(pickup);
            return pickup;
        }

        /// <summary> Применяет силу к Rigidbody предмета, если она задана. </summary>
        /// <param name="pickup"> Объект предмета. </param>
        /// <param name="force"> Сила, применяемая к Rigidbody. </param>
        private static void ApplyForce(Pickup pickup, Vector3 force)
        {
            if (pickup.TryGetComponent(out Rigidbody rigidbody)) rigidbody.AddForce(force, ForceMode.Impulse);
        }

        /// <summary> Извлекает предмет и его количество из слота инвентаря и удаляет его. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="slotIndex"> Индекс слота. </param>
        /// <param name="item"> Возвращаемый предмет. </param>
        /// <param name="quantity"> Возвращаемое количество. </param>
        /// <returns> True, если удалось извлечь предмет; иначе — false. </returns>
        private static bool TryConsumeInventorySlot(Inventory inventory, int slotIndex,
            out InventoryItem item, out int quantity)
        {
            item = inventory.GetItemInSlot(slotIndex);
            quantity = inventory.GetNumberInSlot(slotIndex);

            if (quantity <= 0 || !item)
            {
                Debug.LogError($"[ItemDropService] Slot {slotIndex} is empty.");
                return false;
            }

            inventory.RemoveFromSlot(slotIndex);
            return true;
        }

        #region Saving

        /// <summary> Структура для сериализации информации о выброшенных предметах. </summary>
        [Serializable]
        private struct DropSaveData
        {
            /// <summary> ID предмета, который был выброшен. </summary>
            public string ItemID;

            /// <summary> Позиция, в которой находился выброшенный предмет. </summary>
            public SerializableVector3 Position;

            /// <summary> Количество выброшенных предметов. </summary>
            public int Quantity;
        }

        /// <summary> Сохраняет текущее состояние выброшенных предметов. </summary>
        /// <returns> Список сериализованных данных о предметах. </returns>
        public object CaptureState()
        {
            _spawnedPickups.RemoveAll(pickup => !pickup);
            return _spawnedPickups.Select(p => new DropSaveData
            {
                ItemID = p.Item.ItemID,
                Position = new SerializableVector3(p.transform.position),
                Quantity = p.Number
            }).ToList();
        }

        /// <summary> Восстанавливает выброшенные предметы из сохраненного состояния. </summary>
        /// <param name="state"> Сохраненные данные. </param>
        public void RestoreState(object state)
        {
            if (state is not List<DropSaveData> records) return;

            foreach (var record in records)
            {
                var item = ItemDatabase.GetItemFromID(record.ItemID);
                if (item) Spawn(item, record.Quantity, record.Position.ToVector());
            }
        }

        #endregion
    }
}