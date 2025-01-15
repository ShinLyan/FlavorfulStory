using FlavorfulStory.Saving;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.EquipmentSystem
{
    /// <summary> Хранилище предметов, которыми оснащен игрок. 
    /// Предметы хранятся в специальных местах снаряжения. </summary>
    /// <remarks>  Этот компонент должен быть размещен на объекте "Player". </remarks>
    public class Equipment : MonoBehaviour, ISaveable
    {
        /// <summary> Экипированные предметы. </summary>
        private Dictionary<EquipmentType, EquipableItem> _equippedItems = new();

        /// <summary> Срабатывает, когда предметы в слотах добавляются / удаляются. </summary>
        public event System.Action EquipmentUpdated;

        /// <summary> Получить предмет снаряжения по типу. </summary>
        /// <param name="equipLocation"> Тип снаряжения. </param>
        /// <returns> Возвращает предмет снаряжения по типу. </returns>
        public EquipableItem GetEquipmentFromType(EquipmentType equipLocation) =>
            _equippedItems.TryGetValue(equipLocation, out var item) ? item : null;

        /// <summary> Добавить предмет в указанное место снаряжения. </summary>
        public void AddItem(EquipmentType slot, EquipableItem item)
        {
            _equippedItems[slot] = item;
            EquipmentUpdated?.Invoke();
        }

        /// <summary> Удалить предмет снаряжения по типу. </summary>
        public void RemoveItem(EquipmentType slot)
        {
            _equippedItems.Remove(slot);
            EquipmentUpdated?.Invoke();
        }

        /// <summary> Получить список всех экипированных предметов. </summary>
        public IEnumerable<EquipmentType> GetEquippedItems() => _equippedItems.Keys;

        #region Saving
        /// <summary> Фиксация состояния объекта при сохранении. </summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
        public object CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipmentType, string>();
            foreach (var pair in _equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.ItemID;
            }
            return equippedItemsForSerialization;
        }

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public void RestoreState(object state)
        {
            _equippedItems = new Dictionary<EquipmentType, EquipableItem>();

            var equippedItemsForSerialization = state as Dictionary<EquipmentType, string>;
            foreach (var pair in equippedItemsForSerialization)
            {
                var item = InventoryItem.GetItemFromID(pair.Value) as EquipableItem;
                if (item) _equippedItems[pair.Key] = item;
            }
        }
        #endregion
    }
}