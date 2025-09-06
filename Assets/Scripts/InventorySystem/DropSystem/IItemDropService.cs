using UnityEngine;

namespace FlavorfulStory.InventorySystem.DropSystem
{
    /// <summary> Интерфейс сервиса выброса предметов в игровой мир. </summary>
    public interface IItemDropService
    {
        /// <summary> Выбрасывает предмет в мир в указанной позиции с опциональной силой. </summary>
        /// <param name="itemStack"> Предмет и его количество для выбрасывания. </param>
        /// <param name="position"> Позиция появления предмета. </param>
        /// <param name="force"> Применяемая сила (например, для отталкивания). </param>
        void Drop(ItemStack itemStack, Vector3 position, Vector3? force = null);

        /// <summary> Выбрасывает предмет из конкретного слота инвентаря. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="slotIndex"> Индекс слота, из которого берется предмет. </param>
        /// <param name="position"> Позиция появления предмета. </param>
        /// <param name="force"> Применяемая сила (необязательно). </param>
        void DropFromInventory(Inventory inventory, int slotIndex, Vector3 position, Vector3? force = null);
    }
}