using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    [Serializable]
    public class QuestReward
    {
        [field: SerializeField, Range(1f, 1000f)]
        public int Number { get; private set; }

        [field: SerializeField] public InventoryItem Item { get; private set; }
    }
}