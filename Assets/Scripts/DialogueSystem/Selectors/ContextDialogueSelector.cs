using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI;
using FlavorfulStory.DialogueSystem.Conditions;
using Random = UnityEngine.Random;

namespace FlavorfulStory.DialogueSystem.Selectors
{
    /// <summary> Селектор диалогов, учитывающий контекстные условия. </summary>
    public class ContextDialogueSelector : IDialogueSelector
    {
        /// <summary> Выбирает подходящий диалог для NPC. </summary>
        /// <returns> Выбранный диалог или null. </returns>
        public Dialogue SelectDialogue(NpcInfo npcInfo)
        {
            var dialogues = npcInfo.DialogueConfig.ConditionalDialogues;
            var categoryMap = GroupDialoguesByCategory(dialogues);
            var weightedCategories = CalculateCategoryWeights(categoryMap);
            string chosenCategoryKey = GetRandomCategoryByWeight(weightedCategories);
            return PickRandomDialogueFromCategory(categoryMap[chosenCategoryKey]);
        }

        /// <summary> Группирует диалоги по категорям, основанным на уникальных комбинациях условий. </summary>
        /// <param name="dialogues"> Коллекция условных диалогов для группировки. </param>
        /// <returns> Словарь, где ключами являются категории, а значениями — списки условных диалогов. </returns>
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

        /// <summary> Генерирует ключ для набора условий диалога. </summary>
        /// <param name="conditions"> Коллекция условий, которые используются для создания ключа. </param>
        /// <returns> Строковый ключ, представляющий объединённое и отсортированное представление условий. </returns>
        private static string GetConditionsKey(IEnumerable<DialogueCondition> conditions) =>
            string.Join("|", conditions
                .Where(c => c != null)
                .OrderBy(c => c.ToString())
                .Select(c => c.ToString()));

        /// <summary> Рассчитывает вес для каждой категории диалогов на основе условий. </summary>
        /// <param name="categoryMap"> Словарь, где ключ — имя категории, а значение — список диалогов этой категории. </param>
        /// <returns> Список из пар, каждая из которых содержит категорию и её рассчитанный вес. </returns>
        private static List<(string category, int weight)> CalculateCategoryWeights(
            Dictionary<string, List<ConditionalDialogue>> categoryMap) =>
            categoryMap.Select(pair =>
            {
                int categoryWeight = pair.Value.Sum(conditionalDialogue =>
                    conditionalDialogue.Conditions.Sum(condition => condition.GetWeight()));
                return (category: pair.Key, weight: categoryWeight);
            }).ToList();

        /// <summary> Выбирает случайную категорию с учетом весов. </summary>
        /// <param name="pool"> Список категорий с весами. </param>
        /// <returns> Ключ выбранной категории. </returns>
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

        /// <summary> Выбирает случайный диалог из категории. </summary>
        /// <param name="dialogues"> Список условных диалогов категории. </param>
        /// <returns> Случайный выбранный диалог. </returns>
        private static Dialogue PickRandomDialogueFromCategory(List<ConditionalDialogue> dialogues) =>
            dialogues[Random.Range(0, dialogues.Count)].Dialogue;
    }
}