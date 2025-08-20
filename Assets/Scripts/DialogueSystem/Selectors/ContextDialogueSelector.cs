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
        /// <param name="npcName"> Имя NPC. </param>
        /// <returns> Выбранный диалог или null. </returns>
        public Dialogue SelectDialogue(NpcName npcName)
        {
            var dialogues = DialogueDatabase.GetDialoguesFromNameAndType(npcName, DialogueType.Context);
            var categoryMap = GroupDialoguesByCategory(dialogues);

            if (categoryMap.Count == 0) return null;

            var weightedCategories = CalculateCategoryWeights(categoryMap);
            string chosenCategoryKey = GetRandomCategoryByWeight(weightedCategories);
            return PickRandomDialogueFromCategory(categoryMap[chosenCategoryKey]);
        }

        /// <summary> Группирует диалоги по уникальным комбинациям условий. </summary>
        /// <param name="dialogues"> Список диалогов. </param>
        /// <returns> Словарь сгруппированных диалогов. </returns>
        private static Dictionary<string, List<Dialogue>> GroupDialoguesByCategory(IEnumerable<Dialogue> dialogues)
        {
            var categoryMap = new Dictionary<string, List<Dialogue>>();

            foreach (var dialogue in dialogues)
            {
                if (!DialogueMatchesCurrentState(dialogue)) continue;

                string key = GetConditionsKey(dialogue.Conditions);
                if (!categoryMap.TryGetValue(key, out var list))
                {
                    list = new List<Dialogue>();
                    categoryMap[key] = list;
                }

                list.Add(dialogue);
            }

            return categoryMap;
        }

        /// <summary> Проверяет соответствие условий диалога текущему состоянию. </summary>
        /// <param name="dialogue"> Диалог для проверки. </param>
        /// <returns> Результат проверки. </returns>
        private static bool DialogueMatchesCurrentState(Dialogue dialogue) =>
            dialogue.Conditions.All(c => c.MatchesCurrentState());

        /// <summary> Вычисляет веса для категорий диалогов. </summary>
        /// <param name="categoryMap"> Сгруппированные диалоги. </param>
        /// <returns> Список категорий с весами. </returns>
        private static List<(string category, int weight)> CalculateCategoryWeights(
            Dictionary<string, List<Dialogue>> categoryMap) => categoryMap.Select(pair =>
        {
            int categoryWeight =
                pair.Value.Sum(dialogue => dialogue.Conditions.Sum(condition => condition.GetWeight()));
            return (category: pair.Key, weight: categoryWeight);
        }).ToList();

        /// <summary> Выбирает категорию с учетом весов. </summary>
        /// <param name="pool"> Категории с весами. </param>
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
        /// <param name="dialogues"> Диалоги категории. </param>
        /// <returns> Случайный диалог. </returns>
        private static Dialogue PickRandomDialogueFromCategory(List<Dialogue> dialogues) =>
            dialogues[Random.Range(0, dialogues.Count)];

        /// <summary> Генерирует ключ для набора условий. </summary>
        /// <param name="conditions"> Список условий. </param>
        /// <returns> Уникальный строковый ключ. </returns>
        private static string GetConditionsKey(List<DialogueCondition> conditions) => string.Join("|",
            conditions.OrderBy(condition => condition.ToString()).Select(condition => condition.ToString()));
    }
}