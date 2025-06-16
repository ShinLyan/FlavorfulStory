using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory
{
    public interface IItemDropService
    {
        void Drop(InventoryItem item, int quantity, Vector3 position);
        void Drop(InventoryItem item, int quantity, Vector3 position, Vector3 force);
        void DropFromInventory(Inventory inventory, InventoryItem item, int quantity, Vector3 position);
        void DropFromInventory(Inventory inventory, InventoryItem item, int quantity, Vector3 position, Vector3 force);
    }
}