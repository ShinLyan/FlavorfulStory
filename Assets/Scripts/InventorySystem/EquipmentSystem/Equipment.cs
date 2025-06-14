using System;
using System.Collections.Generic;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.EquipmentSystem
{
    /// <summary> Экипировка игрока. </summary>
    /// <remarks> Позволяет добавлять, удалять и сохранять предметы экипировки. </remarks>
    public class Equipment : MonoBehaviour, ISaveable
    {
        /// <summary> Текущее состояние экипировки, связанное со слотами. </summary>
        private Dictionary<EquipmentType, EquipableItem> _equippedItems = new();

        /// <summary> Событие, вызываемое при изменении состояния экипировки. </summary>
        public event Action EquipmentUpdated;

        /// <summary> Получить предмет из указанного слота экипировки. </summary>
        /// <param name="equipLocation"> Слот экипировки. </param>
        /// <returns> Предмет, находящийся в слоте, или null, если слот пуст. </returns>
        public EquipableItem GetEquipmentFromType(EquipmentType equipLocation) =>
            _equippedItems.GetValueOrDefault(equipLocation);

        /// <summary> Добавить предмет в указанный слот экипировки. </summary>
        /// <param name="slot"> Слот, куда добавляется предмет. </param>
        /// <param name="item"> Экипируемый предмет. </param>
        public void AddItem(EquipmentType slot, EquipableItem item)
        {
            _equippedItems[slot] = item;
            EquipmentUpdated?.Invoke();
        }

        /// <summary> Удалить предмет из указанного слота экипировки. </summary>
        /// <param name="slot"> Слот, из которого удаляется предмет. </param>
        public void RemoveItem(EquipmentType slot)
        {
            _equippedItems.Remove(slot);
            EquipmentUpdated?.Invoke();
        }

        /// <summary> Получить список всех слотов, где есть экипированные предметы. </summary>
        /// <returns> Перечисление слотов с экипированными предметами. </returns>
        public IEnumerable<EquipmentType> GetEquippedItems() => _equippedItems.Keys;

        #region Saving

        /// <summary> Сохранить текущее состояние экипировки. </summary>
        /// <returns> Сериализованные данные об экипировке. </returns>
        public object CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipmentType, string>();
            foreach (var pair in _equippedItems) equippedItemsForSerialization[pair.Key] = pair.Value.ItemID;

            return equippedItemsForSerialization;
        }

        /// <summary> Восстановить состояние экипировки из сохраненных данных. </summary>
        /// <param name="state"> Сохраненные данные экипировки. </param>
        public void RestoreState(object state)
        {
            _equippedItems = new Dictionary<EquipmentType, EquipableItem>();

            var equippedItemsForSerialization = state as Dictionary<EquipmentType, string>;
            foreach (var pair in equippedItemsForSerialization)
            {
                var item = ItemDatabase.GetItemFromID(pair.Value) as EquipableItem;
                if (item) _equippedItems[pair.Key] = item;
            }
        }

        #endregion
    }
}