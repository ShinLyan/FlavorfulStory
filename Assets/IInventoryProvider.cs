using System.Collections.Generic;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory
{
    public interface IInventoryProvider
    {
        IEnumerable<Inventory> GetAll(InventoryType type);
        Inventory GetPlayerInventory();
    }
}