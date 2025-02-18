using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.InventorySystem.UI.Dragging;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        public int GetMaxAcceptableItemsNumber(InventoryItem item) => int.MaxValue;

        public void AddItems(InventoryItem item, int number)
        {
            var itemDropper = GameObject.FindWithTag("Player").GetComponent<ItemDropper>();
            itemDropper.DropItem(item, number);
        }
    }
}