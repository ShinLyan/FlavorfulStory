using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Localization/Localization Database")]
    public class LocalizationDatabase : ScriptableObject
    {
        [SerializeField] private List<LocalizationEntry> _entries = new();

        public Dictionary<string, Dictionary<string, string>> ToDictionary()
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            foreach (var entry in _entries)
            {
                if (!result.ContainsKey(entry.Key)) result[entry.Key] = new Dictionary<string, string>();
                foreach (var pair in entry.Translations) result[entry.Key][pair.LanguageCode] = pair.Text;
            }

            return result;
        }

        public void SetEntries(List<LocalizationEntry> entries) => _entries = entries;
    }

    [Serializable]
    public class LocalizationEntry
    {
        public string Key;
        public List<LocalizedValue> Translations = new();
    }

    [Serializable]
    public class LocalizedValue
    {
        public string LanguageCode;
        public string Text;
    }
}