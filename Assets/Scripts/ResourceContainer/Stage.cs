using System;
using System.Collections.Generic;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Грейд. </summary>
    [Serializable]
    public struct Stage
    {
        /// <summary> Список предметов, выпадающих при разрушении. </summary>
        [field: Tooltip("Список предметов, выпадающих при разрушении."), SerializeField]
        public List<ItemStack> Items { get; private set; }

        /// <summary> Количество ударов для каждой стадии объекта. </summary>
        [field: Tooltip("Количество ударов для перехода на следующую стадию."), SerializeField, Range(1, 5)]
        public int RequiredHits { get; private set; }
    }
}