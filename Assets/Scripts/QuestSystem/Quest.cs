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
        [field: Tooltip("NPC, который выдал квест."), SerializeField]
        public NpcInfo QuestGiver { get; private set; }

        /// <summary> Название квеста. </summary>
        [field: Tooltip("Название квеста."), SerializeField]
        public string QuestName { get; private set; }

        /// <summary> Описание квеста. </summary>
        [field: Tooltip("Описание квеста."), SerializeField, TextArea(5, 10)]
        public string QuestDescription { get; private set; }

        /// <summary> Тип квеста. </summary>
        [field: Tooltip("Тип квеста."), SerializeField]
        public QuestType QuestType { get; private set; }

        /// <summary> Список целей квеста. </summary>
        [Tooltip("Список целей квеста."), SerializeField]
        private List<QuestObjective> _objectives;

        /// <summary> Коллекция целей квеста (только для чтения). </summary>
        public IEnumerable<QuestObjective> Objectives => _objectives;

        /// <summary> Список наград за выполнение квеста. </summary>
        [Tooltip("Список наград за выполнение квеста."), SerializeField]
        private List<QuestReward> _rewards;

        /// <summary> Коллекция наград за квест (только для чтения). </summary>
        public IEnumerable<QuestReward> Rewards => _rewards;

        /// <summary> Кэш всех загруженных квестов. </summary>
        private static Quest[] _cachedQuests;

        /// <summary> Ленивая загрузка и кэширование всех квестов из ресурсов. </summary>
        private static Quest[] CachedQuests
        {
            get
            {
                if (_cachedQuests != null) return _cachedQuests;

                _cachedQuests = Resources.LoadAll<Quest>(string.Empty);
                return _cachedQuests;
            }
        }

        /// <summary> Проверяет, содержит ли квест цель с указанной ссылкой. </summary>
        /// <param name="objectiveReference"> Ссылка на цель. </param>
        /// <returns> True, если цель найдена; иначе — false. </returns>
        public bool HasObjective(string objectiveReference) =>
            Objectives.Any(objective => objective.Reference == objectiveReference);

        /// <summary> Находит квест по его названию. </summary>
        /// <param name="questName"> Название квеста. </param>
        /// <returns> Найденный квест или null, если не найден. </returns>
        public static Quest GetByName(string questName) =>
            CachedQuests.FirstOrDefault(quest => quest.QuestName == questName);
    }
}