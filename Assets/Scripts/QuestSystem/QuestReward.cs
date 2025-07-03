using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Награда за выполнение квеста. </summary>
    [Serializable]
    public class QuestReward
    {
        /// <summary> Количество выдаваемых предметов. </summary>
        [field: SerializeField, Range(1f, 1000f)]
        public int Number { get; private set; }

        /// <summary> Предмет, который выдается в качестве награды. </summary>
        [field: SerializeField] public InventoryItem Item { get; private set; }
    }
}