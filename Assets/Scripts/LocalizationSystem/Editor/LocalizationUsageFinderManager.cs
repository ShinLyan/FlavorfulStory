using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public class LocalizationUsageFinderManager
    {
        private readonly Dictionary<string, ILocalizationUsageFinder> _finders = new();
        private readonly UIPrefabLocalizationUsageFinder _uiFinder;

        public LocalizationUsageFinderManager(UIPrefabLocalizationUsageFinder uiFinder)
        {
            _uiFinder = uiFinder;
            _finders["dialogue"] = new DialogueLocalizationUsageFinder();
            _finders["quest"] = new QuestLocalizationUsageFinder();
            _finders["item"] = new ItemLocalizationUsageFinder();
            _finders["ui"] = _uiFinder;
        }

        public List<Object> FindUsages(string group, string key)
        {
            string lowerGroup = group.ToLower();
            if (_finders.TryGetValue(lowerGroup, out var finder)) return finder.FindUsages(key);

            return new List<Object>();
        }
    }
}