using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Выпадающий предмет и его количество.</summary>
    [System.Serializable]
    public class DropItem
    {
        /// <summary> Предмет, который должен будет выпасть.</summary>
        [Tooltip("Предмет, который должен будет выпасть.")]
        public InventoryItem Item;

        /// <summary> Количество выпадающих предметов.</summary>
        [Tooltip("Количество выпадающих предметов.")]
        [Range(1, 100)]
        public int Quantity;
    }
}