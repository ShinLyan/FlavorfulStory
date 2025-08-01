using System.Collections.Generic;
using FlavorfulStory.QuestSystem;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public class QuestLocalizationUsageFinder : ILocalizationUsageFinder
    {
        private Quest[] _cachedQuests;

        private void LoadQuests() { _cachedQuests = Resources.LoadAll<Quest>(""); }

        public List<Object> FindUsages(string key)
        {
            if (_cachedQuests == null || _cachedQuests.Length == 0) LoadQuests();

            var results = new List<Object>();

            foreach (var quest in _cachedQuests)
            {
                if (quest == null) continue;

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