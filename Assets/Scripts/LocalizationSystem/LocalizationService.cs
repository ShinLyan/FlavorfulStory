using System;
using System.Collections.Generic;

namespace FlavorfulStory.LocalizationSystem
{
    public class LocalizationService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _translationsPerKey;
        private readonly HashSet<string> _languages;
        private Dictionary<string, string> _currentMap;
        private string _currentLang;

        public static LocalizationService Instance { get; private set; }

        public static bool IsInitialized => Instance != null;
        public static string CurrentLanguage => Instance?._currentLang;

        public static event Action OnLanguageChanged;

        public LocalizationService(LocalizationDatabase database, string defaultLang = "EN")
        {
            _translationsPerKey = database.ToDictionary();
            _languages = new HashSet<string>();

            foreach (var entry in _translationsPerKey.Values)
            foreach (string lang in entry.Keys)
                _languages.Add(lang);

            SetLanguage(defaultLang);
            Instance = this;
        }

        private void SetLanguage(string lang)
        {
            _currentLang = _languages.Contains(lang) ? lang : "EN";
            _currentMap = new Dictionary<string, string>();

            foreach (var kvp in _translationsPerKey)
                if (kvp.Value.TryGetValue(_currentLang, out string val))
                    _currentMap[kvp.Key] = val;

            OnLanguageChanged?.Invoke();
        }

        private string Get(string key) =>
            _currentMap.GetValueOrDefault(key, "");

        public static void SetLanguageStatic(string lang) => Instance?.SetLanguage(lang);

        public static string GetLocalizedString(string key) => Instance?.Get(key.TrimStart('[').TrimEnd(']')) ?? "";

        public static IEnumerable<string> GetAllLanguages() => Instance?._languages;
    }
}