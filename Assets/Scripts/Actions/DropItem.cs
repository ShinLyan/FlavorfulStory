using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Выпадающий предмет и его количество.</summary>
    [System.Serializable]
    public class DropItem
    {
        /// <summary> Префаб предмета, который будет падать.</summary>
        [Tooltip("Префаб предмета, который будет падать.")]
        public InventoryItem ItemPrefab;

        /// <summary> Количество выпадающих предметов.</summary>
        [Tooltip("Количество выпадающих предметов.")]
        [Range(1, 100)]
        public int Quantity;
    }
}