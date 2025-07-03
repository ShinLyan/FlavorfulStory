using FlavorfulStory.AI;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Данные квеста, хранящиеся в ScriptableObject. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Quest")]
    public class Quest : ScriptableObject
    {
        /// <summary> Название квеста. </summary>
        [field: SerializeField] public string QuestName { get; private set; }

        /// <summary> Описание квеста. </summary>
        [field: SerializeField] public string QuestDescription { get; private set; }

        /// <summary> Тип квеста: General / Personal / Daily / Weekly / Completed. </summary>
        [field: SerializeField] public string QuestType { get; private set; }

        /// <summary> Список целей квеста. </summary>
        [field: SerializeField] public string[] Objectives { get; private set; }

        /// <summary> NPC, который выдал квест. </summary>
        [field: SerializeField] public NpcInfo QuestGiver { get; private set; }
    }
}