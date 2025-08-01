using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory
{
    public class InventoryProvider : IInventoryProvider
    {
        private readonly List<Inventory> _inventories;

        public InventoryProvider(List<Inventory> inventories) => _inventories = inventories;

        public IEnumerable<Inventory> GetAll(InventoryType type) =>
            _inventories.Where(i => i.Type == type);

        public Inventory GetPlayerInventory() =>
            _inventories.FirstOrDefault(i => i.Type == InventoryType.Player);
    }
}