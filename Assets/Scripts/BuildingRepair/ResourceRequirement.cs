using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Структура, определяющая требование ресурса для ремонта. </summary>
    [Serializable]
    public struct ResourceRequirement
    {
        /// <summary> Требуемый предмет из инвентаря. </summary>
        [Tooltip("Требуемый ресурс.")] public InventoryItem Item;

        /// <summary> Необходимое количество предмета. </summary>
        [Tooltip("Количество требуемого ресурса.")]
        public int Quantity;
    }
}