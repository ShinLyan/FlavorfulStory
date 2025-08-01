using System.Collections.Generic;
using FlavorfulStory.QuestSystem;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> Поисковик использования ключей локализации в квестах и их задачах. </summary>
    public class QuestLocalizationUsageFinder : ILocalizationUsageFinder
    {
        /// <summary> Кэшированный массив всех квестов в проекте. </summary>
        private Quest[] _cachedQuests;

        /// <summary> Загружает все квесты из папки Resources. </summary>
        private void LoadQuests() => _cachedQuests = Resources.LoadAll<Quest>("");

        /// <summary> Находит все использования ключа локализации в квестах. </summary>
        /// <param name="key"> Ключ локализации для поиска. </param>
        /// <returns> Список квестов, содержащих указанный ключ в названии, описании или задачах. </returns>
        public List<Object> FindUsages(string key)
        {
            if (_cachedQuests == null || _cachedQuests.Length == 0) LoadQuests();

            var results = new List<Object>();

            foreach (var quest in _cachedQuests)
            {
                if (!quest) continue;

                if (quest.QuestName.Contains($"[{key}]") || quest.QuestDescription.Contains($"[{key}]"))
                    results.Add(quest);

                foreach (var stage in quest.Stages)
                foreach (var objective in stage.Objectives)
                    if (objective.Description.Contains($"[{key}]"))
                        results.Add(quest);
            }

            return results;
        }
    }
}