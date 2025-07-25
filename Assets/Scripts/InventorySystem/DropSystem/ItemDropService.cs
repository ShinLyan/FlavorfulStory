﻿using System;
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
        /// <param name="itemStack"> Предмет и его количество для выбрасывания. </param>
        /// <param name="position"> Позиция появления предмета. </param>
        /// <param name="force"> Применяемая сила (например, для отталкивания). </param>
        public void Drop(ItemStack itemStack, Vector3 position, Vector3? force = null)
        {
            var pickup = Spawn(itemStack, position, PickupDelay);
            if (pickup && force.HasValue) ApplyForce(pickup, force.Value);
        }

        /// <summary> Выбрасывает предмет из конкретного слота инвентаря. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="slotIndex"> Индекс слота, из которого берется предмет. </param>
        /// <param name="position"> Позиция появления предмета. </param>
        /// <param name="force"> Применяемая сила (необязательно). </param>
        public void DropFromInventory(Inventory inventory, int slotIndex, Vector3 position, Vector3? force = null)
        {
            if (!TryConsumeInventorySlot(inventory, slotIndex, out var itemStack)) return;
            Drop(itemStack, position, force);
        }

        /// <summary> Устанавливает контейнер, в котором будут размещаться выброшенные предметы. </summary>
        /// <param name="container"> Объект-контейнер на сцене. </param>
        public void SetDroppedItemsContainer(Transform container) => _container = container;

        /// <summary> Спавнит предмет в мире с заданным количеством и задержкой. </summary>
        /// <param name="itemStack"> Предмет для спавна. </param>
        /// <param name="position"> Позиция появления. </param>
        /// <param name="pickupDelay"> Задержка перед возможностью поднятия. </param>
        /// <returns> Ссылка на созданный Pickup. </returns>
        private Pickup Spawn(ItemStack itemStack, Vector3 position, float pickupDelay = 1f)
        {
            if (!_container) Debug.LogError($"{nameof(ItemDropService)}.{nameof(Spawn)}: Container is not set)");

            var pickup = _pickupFactory.Create(itemStack, position, pickupDelay, _container);
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
        /// <param name="itemStack"> Возвращаемый предмет и его количество. </param>
        /// <returns> True, если удалось извлечь предмет; иначе — false. </returns>
        private static bool TryConsumeInventorySlot(Inventory inventory, int slotIndex,
            out ItemStack itemStack)
        {
            itemStack = inventory.GetItemStackInSlot(slotIndex);

            if (itemStack.Number <= 0 || !itemStack.Item)
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
                var itemStack = new ItemStack { Item = item, Number = record.Quantity };
                if (item) Spawn(itemStack, record.Position.ToVector());
            }
        }

        #endregion
    }
}