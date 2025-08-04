namespace FlavorfulStory.InventorySystem
{
    /// <summary> Сервис переноса предметов между инвентарями. </summary>
    public class InventoryTransferService
    {
        /// <summary> Переносит указанный стак предметов из одного инвентаря в другой. </summary>
        /// <param name="from">Источник (откуда брать).</param>
        /// <param name="to">Цель (куда класть).</param>
        /// <param name="stack">Стак предметов для переноса.</param>
        public void Transfer(Inventory from, Inventory to, ItemStack stack)
        {
            if (!from.HasItem(stack.Item)) return;
            from.RemoveItem(stack.Item, stack.Number);
            to.TryAddToFirstAvailableSlot(stack.Item, stack.Number);
        }
    }
}