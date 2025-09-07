using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI;
using FlavorfulStory.DialogueSystem.Conditions;
using FlavorfulStory.TimeManagement;
using DateTime = FlavorfulStory.TimeManagement.DateTime;
using Random = UnityEngine.Random;

namespace FlavorfulStory.DialogueSystem.Selectors
{
    /// <summary> Селектор диалогов, учитывающий контекстные условия и ограничение разговоров с NPC. </summary>
    public class ContextDialogueSelector : IDialogueSelector, IInitializableSelector
    {
        // Храним количество разговоров с каждым NPC за текущий день
        private readonly Dictionary<NpcName, int> _dailyNpcConversations = new();

        /// <summary> Инициализирует обработчики событий. </summary>
        public void Initialize() => WorldTime.OnDayEnded += ResetDailyCounters;

        /// <summary> Освобождает ресурсы и отписывается от событий. </summary>
        public void Dispose() => WorldTime.OnDayEnded -= ResetDailyCounters;

        /// <summary> Сброс счетчиков разговоров в конце дня. </summary>
        private void ResetDailyCounters(DateTime _) => _dailyNpcConversations.Clear();

        /// <summary> Выбирает подходящий диалог для NPC. </summary>
        /// <returns> Выбранный диалог или null. </returns>
        public Dialogue SelectDialogue(NpcInfo npcInfo)
        {
            if (_dailyNpcConversations.TryGetValue(npcInfo.NpcName, out int count) && count >= 3) return null;

            var dialogues = npcInfo.DialogueConfig.ConditionalDialogues;
            var categoryMap = GroupDialoguesByCategory(dialogues);

            if (categoryMap.Count == 0) return null;

            var weightedCategories = CalculateCategoryWeights(categoryMap);
            string chosenCategoryKey = GetRandomCategoryByWeight(weightedCategories);
            var dialogue = PickRandomDialogueFromCategory(categoryMap[chosenCategoryKey]);

            _dailyNpcConversations.TryAdd(npcInfo.NpcName, 0);
            _dailyNpcConversations[npcInfo.NpcName]++;

            return dialogue;
        }

        private static Dictionary<string, List<ConditionalDialogue>> GroupDialoguesByCategory(
            IEnumerable<ConditionalDialogue> dialogues)
        {
            var categoryMap = new Dictionary<string, List<ConditionalDialogue>>();

            foreach (var conditionalDialogue in dialogues)
            {
                bool allMatched = conditionalDialogue.Conditions.All(c => c != null && c.MatchesCurrentState());
                if (!allMatched) continue;

                string key = GetConditionsKey(conditionalDialogue.Conditions);

                if (!categoryMap.TryGetValue(key, out var list))
                {
                    list = new List<ConditionalDialogue>();
                    categoryMap[key] = list;
                }

                list.Add(conditionalDialogue);
            }

            return categoryMap;
        }

        private static string GetConditionsKey(IEnumerable<DialogueCondition> conditions) =>
            string.Join("|", conditions
                .Where(c => c != null)
                .OrderBy(c => c.ToString())
                .Select(c => c.ToString()));

        private static List<(string category, int weight)> CalculateCategoryWeights(
            Dictionary<string, List<ConditionalDialogue>> categoryMap) =>
            categoryMap.Select(pair =>
            {
                int categoryWeight = pair.Value.Sum(conditionalDialogue =>
                    conditionalDialogue.Conditions.Sum(condition => condition.GetWeight()));
                return (category: pair.Key, weight: categoryWeight);
            }).ToList();

        private static string GetRandomCategoryByWeight(List<(string category, int weight)> pool)
        {
            int totalWeight = pool.Sum(valueTuple => valueTuple.weight);
            int randomValue = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach ((string category, int weight) in pool)
            {
                cumulative += weight;
                if (randomValue < cumulative) return category;
            }

            return pool[0].category;
        }

        private static Dialogue PickRandomDialogueFromCategory(List<ConditionalDialogue> dialogues) =>
            dialogues[Random.Range(0, dialogues.Count)].Dialogue;
    }
}