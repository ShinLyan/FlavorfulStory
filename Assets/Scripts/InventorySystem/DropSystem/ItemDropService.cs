using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.PickupSystem;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.DropSystem
{
    /// <summary> Сервис, отвечающий за выброс предметов из инвентаря в игровом мире. </summary>
    public class ItemDropService : IItemDropService, ISaveableService
    {
        /// <summary> Сила, с которой выбрасываются ресурсы вверх при спавне. </summary>
        private const float ResourceDropForceValue = 5f;

        /// <summary> Вектор силы, прикладываемый при выбросе ресурса (по умолчанию — вверх). </summary>
        public static readonly Vector3 ResourceDropForce = Vector3.up * ResourceDropForceValue;

        /// <summary> Контейнер, в котором спавнятся все выброшенные предметы. </summary>
        private readonly Transform _container;

        /// <summary> Фабрика создания объектов Pickup'ов. </summary>
        private readonly IPrefabFactory<Pickup> _pickupFactory;

        /// <summary> Список заспавненных Pickup для сохранения и очистки. </summary>
        private readonly List<Pickup> _spawnedPickups = new();

        /// <summary> Конструктор сервиса выброса предметов. </summary>
        /// <param name="pickupFactory"> Фабрика создания Pickup объектов. </param>
        /// <param name="container"> Контейнер, в котором спавнятся все выброшенные предметы. </param>
        public ItemDropService(IPrefabFactory<Pickup> pickupFactory, Transform container)
        {
            _pickupFactory = pickupFactory;
            _container = container;
        }

        /// <summary> Выбрасывает предмет в мир в указанной позиции с опциональной силой. </summary>
        /// <param name="itemStack"> Предмет и его количество для выбрасывания. </param>
        /// <param name="position"> Позиция появления предмета. </param>
        /// <param name="force"> Применяемая сила (например, для отталкивания). </param>
        public void Drop(ItemStack itemStack, Vector3 position, Vector3? force = null)
        {
            var pickup = Spawn(itemStack, position);
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

        /// <summary> Спавнит предмет в мире с заданным количеством и задержкой. </summary>
        /// <param name="itemStack"> Предмет для спавна. </param>
        /// <param name="position"> Позиция появления. </param>
        /// <param name="pickupDelay"> Задержка перед возможностью поднятия. </param>
        /// <returns> Ссылка на созданный Pickup. </returns>
        private Pickup Spawn(ItemStack itemStack, Vector3 position)
        {
            var pickup = _pickupFactory.Create(itemStack.Item.PickupPrefab, position, parentTransform: _container);
            pickup.Setup(itemStack);
            _spawnedPickups.Add(pickup);
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

        #region ISaveable

        /// <summary> Структура для сериализации информации о выброшенных предметах. </summary>
        [Serializable]
        private readonly struct DropRecord
        {
            /// <summary> ID предмета, который был выброшен. </summary>
            public string ItemId { get; }

            /// <summary> Позиция, в которой находился выброшенный предмет. </summary>
            public SerializableVector3 Position { get; }

            /// <summary> Количество выброшенных предметов. </summary>
            public int Quantity { get; }

            /// <summary> Конструктор с параметрами. </summary>
            /// <param name="itemId"> ID предмета, который был выброшен. </param>
            /// <param name="position"> Позиция, в которой находился выброшенный предмет. </param>
            /// <param name="quantity"> Количество выброшенных предметов. </param>
            public DropRecord(string itemId, SerializableVector3 position, int quantity)
            {
                ItemId = itemId;
                Position = position;
                Quantity = quantity;
            }
        }

        /// <summary> Сохраняет текущее состояние выброшенных предметов. </summary>
        /// <returns> Список сериализованных данных о предметах. </returns>
        public object CaptureState()
        {
            _spawnedPickups.RemoveAll(pickup => !pickup);
            return _spawnedPickups.Select(pickup => new DropRecord(pickup.Item.ItemID,
                new SerializableVector3(pickup.transform.position), pickup.Number)).ToList();
        }

        /// <summary> Восстанавливает выброшенные предметы из сохраненного состояния. </summary>
        /// <param name="state"> Сохраненные данные. </param>
        public void RestoreState(object state)
        {
            if (state is not List<DropRecord> records) return;

            foreach (var record in records)
                Spawn(new ItemStack(ItemDatabase.GetItemFromID(record.ItemId), record.Quantity),
                    record.Position.ToVector());
        }

        #endregion
    }
}