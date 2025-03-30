using System;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Слот инвентаря, содержащий предмет и его количество. </summary>
    [Serializable]
    public struct InventorySlot
    {
        /// <summary> Предмет, находящийся в этом слоте. </summary>
        [field: SerializeField]
        public InventoryItem Item { get; set; }

        /// <summary> Количество предметов в этом слоте. </summary>
        [field: SerializeField, Min(1f)]
        public int Number { get; set; }
    }
}