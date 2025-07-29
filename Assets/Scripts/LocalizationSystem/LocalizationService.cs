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

        // === Статический доступ ===

        public static LocalizationService Instance { get; private set; }

        public static bool IsInitialized => Instance != null;
        public static string CurrentLanguage => Instance?._currentLang;

        public static IReadOnlyList<string> AvailableLanguages =>
            Instance != null ? new List<string>(Instance._languages) : new List<string>();


        public event Action OnLanguageChanged;

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

        public void SetLanguage(string lang)
        {
            _currentLang = _languages.Contains(lang) ? lang : "EN";
            _currentMap = new Dictionary<string, string>();

            foreach (var kvp in _translationsPerKey)
                if (kvp.Value.TryGetValue(_currentLang, out string val))
                    _currentMap[kvp.Key] = val;

            OnLanguageChanged?.Invoke();
        }

        public string Get(string key) =>
            _currentMap.GetValueOrDefault(key, "");

        public string GetLocalizedValueOrNull(string key, string lang)
        {
            return _translationsPerKey.TryGetValue(key, out var map) && map.TryGetValue(lang, out string val)
                ? val
                : null;
        }

        public static void SetLanguageStatic(string lang) => Instance?.SetLanguage(lang);

        public static string GetStatic(string key) => Instance?.Get(key) ?? "";

        public static string GetLocalizedValueStatic(string key, string lang) =>
            Instance?.GetLocalizedValueOrNull(key, lang);
    }
}