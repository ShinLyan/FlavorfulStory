using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

public class ItemAddTest : MonoBehaviour
{
    public InventoryItem item;
    private void Start()
    {
        Inventory.PlayerInventory.TryAddToFirstAvailableSlot(item, 1);
    }
}