using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory
{
    //TODO: сделать ревизию pickupDelay и force. Мб Зашить жесткие значения.
    /// <summary> Интерфейс сервиса выброса предметов в игровой мир. </summary>
    public interface IItemDropService
    {
        /// <summary> Выбросить предмет в указанную позицию с задержкой на подбор. </summary>
        void Drop(InventoryItem item, int quantity, Vector3 position, float pickupDelay = 1f);
        /// <summary> Выбросить предмет с силой (например, при броске) и задержкой. </summary>
        void Drop(InventoryItem item, int quantity, Vector3 position, Vector3 force, float pickupDelay = 1f);
        /// <summary> Выбросить предмет из указанного слота инвентаря. </summary>
        void DropFromInventory(Inventory inventory, int slotIndex, Vector3 position, float pickupDelay = 1f);
        /// <summary> Выбросить предмет из инвентаря с применением силы. </summary>
        void DropFromInventory(Inventory inventory, int slotIndex, Vector3 position, Vector3 force, float pickupDelay = 1f);
        /// <summary> Установить родительский объект (контейнер) для всех выброшенных предметов. </summary>
        void SetDroppedItemsContainer(Transform container);
    }
}