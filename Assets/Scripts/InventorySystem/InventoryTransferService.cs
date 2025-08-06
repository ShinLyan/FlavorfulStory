using System.Collections.Generic;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Сервис переноса предметов между инвентарями. </summary>
    public class InventoryTransferService
    {
        /// <summary> Получить список стакающихся предметов, которые можно перенести из одного инвентаря в другой. </summary>
        /// <param name="from"> Инвентарь, из которого происходит перенос. </param>
        /// <param name="to"> Инвентарь, в который происходит перенос. </param>
        /// <returns> Список пар (индекс слота, предмет), которые были успешно добавлены. </returns>
        public List<(int SlotIndex, ItemStack Stack)> GetStackablesToTransfer(Inventory from, Inventory to)
        {
            var result = new List<(int, ItemStack)>();
            for (int i = 0; i < from.InventorySize; i++)
            {
                var itemStack = from.GetItemStackInSlot(i);
                var item = itemStack.Item;
                int number = itemStack.Number;

                if (!item || !item.IsStackable || itemStack.Number <= 0 || !to.HasItem(item) || !to.HasSpaceFor(item))
                    continue;

                if (to.TryAddToFirstAvailableSlot(item, number)) result.Add((i, itemStack));
            }

            return result;
        }
    }
}