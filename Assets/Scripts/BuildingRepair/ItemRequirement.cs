using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.BuildingRepair
{
    //TODO: Вынести из неймспейса BuildingRepair в более логичное место.
    /// <summary> Структура, определяющая требование предметов. </summary>
    /// <remarks> Используется в <see cref="RepairStage"/> и <see cref="CraftingRecipe"/>. </remarks>
    [Serializable]
    public struct ItemRequirement
    {
        /// <summary> Требуемый предмет из инвентаря. </summary>
        [Tooltip("Требуемый предмет.")] 
        public InventoryItem Item;

        /// <summary> Необходимое количество предмета. </summary>
        [Tooltip("Количество требуемого ресурса.")]
        public int Quantity;
    }
}