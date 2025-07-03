using System;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    [Serializable]
    public class QuestObjective
    {
        [field: SerializeField] public string Reference { get; private set; }

        [field: SerializeField] public string Description { get; private set; }
    }
}