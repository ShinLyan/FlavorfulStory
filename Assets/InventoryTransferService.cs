using FlavorfulStory.InventorySystem;

namespace FlavorfulStory
{
    public class InventoryTransferService
    {
        public void Transfer(Inventory from, Inventory to, ItemStack stack)
        {
            if (!from.HasItem(stack.Item)) return;
            from.RemoveItem(stack.Item, stack.Number);
            to.TryAddToFirstAvailableSlot(stack.Item, stack.Number);
        }
    }
}