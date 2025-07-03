using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Данные квеста, хранящиеся в ScriptableObject. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Quest")]
    public class Quest : ScriptableObject
    {
        /// <summary> NPC, который выдал квест. </summary>
        [field: SerializeField] public NpcInfo QuestGiver { get; private set; }

        /// <summary> Название квеста. </summary>
        [field: SerializeField] public string QuestName { get; private set; }

        /// <summary> Описание квеста. </summary>
        [field: SerializeField] public string QuestDescription { get; private set; }

        /// <summary> Тип квеста: General / Personal / Daily / Weekly / Completed. </summary>
        [field: SerializeField] public string QuestType { get; private set; }

        /// <summary> Список целей квеста. </summary>
        [SerializeField] private List<QuestObjective> _objectives;

        public IEnumerable<QuestObjective> Objectives => _objectives;


        [SerializeField] private List<QuestReward> _rewards;
        public IEnumerable<QuestReward> Rewards => _rewards;


        private static Quest[] _cachedQuests;

        private static Quest[] CachedQuests
        {
            get
            {
                if (_cachedQuests != null) return _cachedQuests;

                _cachedQuests = Resources.LoadAll<Quest>(string.Empty);
#if UNITY_EDITOR
                Debug.Log($"[Quest] Cached {_cachedQuests.Length} quests.");
#endif

                return _cachedQuests;
            }
        }

        public bool HasObjective(string objectiveReference) =>
            Objectives.Any(objective => objective.Reference == objectiveReference);

        public static Quest GetByName(string questName) =>
            CachedQuests.FirstOrDefault(quest => quest.QuestName == questName);
    }
}