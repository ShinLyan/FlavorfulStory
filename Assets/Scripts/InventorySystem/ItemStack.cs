using System;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Стак предметов. </summary>
    [Serializable]
    public struct ItemStack
    {
        /// <summary> Предмет. </summary>
        [field: SerializeField] public InventoryItem Item { get; set; }

        /// <summary> Количество предметов. </summary>
        [field: SerializeField, Min(1f)] public int Number { get; set; }
    }
}