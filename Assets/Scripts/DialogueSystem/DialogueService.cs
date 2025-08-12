using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI;
using FlavorfulStory.DialogueSystem.Conditions;
using Random = UnityEngine.Random;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Сервис для работы с системой диалогов. </summary>
    public class DialogueService
    {
        /// <summary> Конфигурация весов для условий диалогов. </summary>
        private readonly DialogueWeightsConfig _weightsConfig;

        /// <summary> Инициализирует сервис с конфигурацией весов. </summary>
        /// <param name="weightsConfig"> Конфигурация весов условий. </param>
        public DialogueService(DialogueWeightsConfig weightsConfig) => _weightsConfig = weightsConfig;

        /// <summary> Получает случайный взвешенный диалог для NPC. </summary>
        /// <param name="npcName"> Имя NPC. </param>
        /// <returns> Случайный диалог или null, если нет подходящих. </returns>
        public Dialogue GetRandomWeightedDialogue(NpcName npcName)
        {
            var dialogues = DialogueDatabase.GetDialoguesFromName(npcName);
            var categoryMap = GroupDialoguesByCategory(dialogues);

            if (categoryMap.Count == 0) return null;

            var weightedCategories = CalculateCategoryWeights(categoryMap);
            string chosenCategoryKey = GetRandomCategoryByWeight(weightedCategories);
            return PickRandomDialogueFromCategory(categoryMap[chosenCategoryKey]);
        }

        /// <summary> Группирует диалоги по уникальным комбинациям условий. </summary>
        /// <param name="dialogues"> Список диалогов для группировки. </param>
        /// <returns> Словарь категорий диалогов. </returns>
        private Dictionary<string, List<Dialogue>> GroupDialoguesByCategory(IEnumerable<Dialogue> dialogues)
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
        /// <returns> True если все условия выполнены. </returns>
        private static bool DialogueMatchesCurrentState(Dialogue dialogue) =>
            dialogue.Conditions.All(c => c.MatchesCurrentState());

        /// <summary> Вычисляет суммарные веса для каждой категории. </summary>
        /// <param name="categoryMap"> Сгруппированные диалоги. </param>
        /// <returns> Список категорий с весами. </returns>
        private List<(string category, int weight)> CalculateCategoryWeights(
            Dictionary<string, List<Dialogue>> categoryMap)
        {
            return categoryMap
                .Select(kvp =>
                {
                    int categoryWeight = kvp.Value
                        .Sum(d => d.Conditions.Sum(c => c.GetWeight(_weightsConfig)));
                    return (category: kvp.Key, weight: categoryWeight);
                })
                .ToList();
        }

        /// <summary> Выбирает категорию с учетом весов. </summary>
        /// <param name="pool"> Категории с весами. </param>
        /// <returns> Ключ выбранной категории. </returns>
        private static string GetRandomCategoryByWeight(List<(string category, int weight)> pool)
        {
            int totalWeight = pool.Sum(p => p.weight);
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
        private static string GetConditionsKey(List<DialogueCondition> conditions) =>
            string.Join("|", conditions
                .OrderBy(c => c.ToString())
                .Select(c => c.ToString()));
    }
}