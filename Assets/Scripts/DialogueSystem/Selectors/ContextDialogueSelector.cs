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
    public class ContextDialogueSelector : IInitializableSelector
    {
        /// <summary> Храним количество разговоров с каждым NPC за текущий день. </summary>
        private readonly Dictionary<NpcName, int> _dailyNpcConversations = new();

        /// <summary> Словарь, содержащий информацию о диалогах, воспроизведенных сегодня для каждого NPC. </summary>
        private readonly Dictionary<NpcName, HashSet<Dialogue>> _playedDialoguesToday = new();

        /// <summary> Инициализирует обработчики событий. </summary>
        public void Initialize() => WorldTime.OnDayEnded += ResetDailyCounters;

        /// <summary> Освобождает ресурсы и отписывается от событий. </summary>
        public void Dispose() => WorldTime.OnDayEnded -= ResetDailyCounters;

        /// <summary> Сброс счетчиков разговоров в конце дня. </summary>
        /// <param name="_"> Текущее игровое время (не используется). </param>
        private void ResetDailyCounters(DateTime _)
        {
            _dailyNpcConversations.Clear();
            _playedDialoguesToday.Clear();
        }

        /// <summary> Выбирает подходящий диалог для NPC. </summary>
        /// <param name="npcInfo"> Информация об NPC, включая конфигурацию диалогов. </param>
        /// <returns> Подходящий диалог или null, если не найден. </returns>
        public Dialogue SelectDialogue(NpcInfo npcInfo)
        {
            if (_dailyNpcConversations.TryGetValue(npcInfo.NpcName, out int count) && count >= 3) return null;

            var categoryMap = GroupDialoguesByCategory(npcInfo.DialogueConfig.ContextDialogues);

            if (categoryMap.Count == 0) return null;

            RemovePlayedDialoguesFromCategories(npcInfo.NpcName, categoryMap);

            if (categoryMap.Count == 0) return null;

            var weightedCategories = CalculateCategoryWeights(categoryMap);
            string chosenCategoryKey = GetRandomCategoryByWeight(weightedCategories);
            var dialogue = PickRandomDialogueFromCategory(categoryMap[chosenCategoryKey], npcInfo.NpcName);

            _dailyNpcConversations.TryAdd(npcInfo.NpcName, 0);
            _dailyNpcConversations[npcInfo.NpcName]++;

            return dialogue;
        }

        /// <summary> Группирует диалоги по ключу условий, оставляя только удовлетворяющие условиям. </summary>
        /// <param name="dialogues"> Список всех контекстных диалогов. </param>
        /// <returns> Словарь: ключ условий -> список диалогов. </returns>
        private static Dictionary<string, List<ContextDialogue>> GroupDialoguesByCategory(
            IEnumerable<ContextDialogue> dialogues)
        {
            var categoryMap = new Dictionary<string, List<ContextDialogue>>();

            foreach (var contextDialogue in dialogues)
            {
                if (!contextDialogue.Conditions.All(condition => condition is { IsSatisfied: true })) continue;

                string key = GetConditionsKey(contextDialogue.Conditions);
                if (!categoryMap.TryGetValue(key, out var list))
                {
                    list = new List<ContextDialogue>();
                    categoryMap[key] = list;
                }

                list.Add(contextDialogue);
            }

            return categoryMap;
        }

        /// <summary> Удаляет из категорий диалоги, которые уже были воспроизведены сегодня для указанного NPC </summary>
        /// <param name="npcName"> Имя NPC, для которого выполняется фильтрация диалогов. </param>
        /// <param name="categoryMap"> Словарь категорий диалогов. </param>
        private void RemovePlayedDialoguesFromCategories(NpcName npcName,
            Dictionary<string, List<ContextDialogue>> categoryMap)
        {
            foreach (var key in categoryMap.Keys.ToList())
            {
                var played = _playedDialoguesToday.GetValueOrDefault(npcName, new HashSet<Dialogue>());

                categoryMap[key] = categoryMap[key].Where(contextDialogue =>
                    contextDialogue.Dialogues.Any(dialogue => !played.Contains(dialogue))).ToList();

                if (categoryMap[key].Count == 0) categoryMap.Remove(key);
            }
        }

        /// <summary> Формирует строковый ключ из условий (по их ToString), сортируя их. </summary>
        /// <param name="conditions"> Коллекция условий. </param>
        /// <returns> Строка, представляющая уникальный ключ набора условий. </returns>
        private static string GetConditionsKey(IEnumerable<DialogueCondition> conditions) => string.Join("|",
            conditions.Where(condition => condition != null).OrderBy(condition => condition.ToString())
                .Select(condition => condition.ToString()));

        /// <summary> Подсчитывает общий вес диалогов в каждой категории. </summary>
        /// <param name="categoryMap"> Словарь категорий и диалогов. </param>
        /// <returns> Список пар: категория и её суммарный вес. </returns>
        private static List<(string category, int weight)> CalculateCategoryWeights(
            Dictionary<string, List<ContextDialogue>> categoryMap) =>
            categoryMap.Select(pair =>
            {
                int categoryWeight = pair.Value.Sum(conditionalDialogue =>
                    conditionalDialogue.Conditions.Sum(condition => condition.Weight));
                return (category: pair.Key, weight: categoryWeight);
            }).ToList();

        /// <summary> Выбирает случайную категорию с учетом веса. </summary>
        /// <param name="pool"> Список категорий с весами. </param>
        /// <returns> Имя выбранной категории. </returns>
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
        /// <param name="dialogues"> Список диалогов внутри категории. </param>
        /// <param name="npcName"> Имя NPC. </param>
        /// <returns> Один из диалогов в категории. </returns>
        private Dialogue PickRandomDialogueFromCategory(List<ContextDialogue> dialogues, NpcName npcName)
        {
            var playedDialogues = _playedDialoguesToday.GetValueOrDefault(npcName, new HashSet<Dialogue>());

            var nonPlayedDialogues = dialogues.SelectMany(d => d.Dialogues)
                .Where(dialogue => !playedDialogues.Contains(dialogue)).ToList();

            if (nonPlayedDialogues.Count == 0) return null;

            var chosenDialogue = nonPlayedDialogues[Random.Range(0, nonPlayedDialogues.Count)];

            if (!_playedDialoguesToday.ContainsKey(npcName)) _playedDialoguesToday[npcName] = new HashSet<Dialogue>();

            _playedDialoguesToday[npcName].Add(chosenDialogue);

            return chosenDialogue;
        }
    }
}