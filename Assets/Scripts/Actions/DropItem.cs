using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Данные о предмете, выпадающем при взаимодействии. </summary>
    [System.Serializable]
    public class DropItem
    {
        /// <summary> Префаб предмета, который будет создан. </summary>
        [Tooltip("Префаб предмета, который будет создан.")]
        public InventoryItem ItemPrefab;

        /// <summary> Количество выпадающих предметов. </summary>
        [Tooltip("Количество выпадающих предметов.")]
        [Range(1, 100)]
        public int Quantity;
    }
}