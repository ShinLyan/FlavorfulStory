using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> Сервис для получения переведённых строк по ключу. </summary>
    public class LocalizationService : IDisposable
    {
        private readonly Dictionary<string, LocalizationData> _allLanguages;
        private Dictionary<string, string> _currentMap;
        private string _currentLang;

        private static LocalizationService _instance;

        public static string CurrentLanguage => _instance?._currentLang;

        public static bool IsInitialized => _instance != null;

        public static IReadOnlyDictionary<string, Dictionary<string, string>> AllTranslations
        {
            get
            {
                if (_instance == null) return null;

                var result = new Dictionary<string, Dictionary<string, string>>();

                foreach (var langPair in _instance._allLanguages)
                {
                    string lang = langPair.Key;
                    var translations = langPair.Value.Translations;

                    foreach (var kvp in translations)
                    {
                        string key = kvp.Key;
                        string value = kvp.Value;

                        if (!result.ContainsKey(key)) result[key] = new Dictionary<string, string>();

                        result[key][lang] = value;
                    }
                }

                return result;
            }
        }

        public static List<string> AvailableLanguages =>
            _instance != null ? new List<string>(_instance._allLanguages.Keys) : new List<string>();


        public LocalizationService(Dictionary<string, LocalizationData> allLanguages, string defaultLang = "en")
        {
            _allLanguages = allLanguages;
            SetLanguage(defaultLang);

            _instance = this;
        }

        public void Dispose()
        {
            if (_instance == this) _instance = null;
        }

        public static void SetLanguage(string lang)
        {
            _instance?.SetLanguageInternal(lang);
            UILocalizer.RefreshAll();
        }

        private void SetLanguageInternal(string lang)
        {
            if (_allLanguages.TryGetValue(lang, out var data))
            {
                _currentLang = lang;
                _currentMap = data.Translations;
            }
            else
            {
                Debug.LogWarning($"Language '{lang}' not found. Fallback to English.");
                _currentLang = "en";
                _currentMap = _allLanguages.ContainsKey("en")
                    ? _allLanguages["en"].Translations
                    : new Dictionary<string, string>();
            }
        }

        public static string Get(string key)
        {
            if (_instance == null) return $"#{key}#";

            return _instance._currentMap.TryGetValue(key, out string val) ? val : $"#{key}#";
        }

        public static string GetLocalizedValueOrNull(string key, string lang)
        {
            if (_instance == null) return null;

            if (_instance._allLanguages.TryGetValue(lang, out var data))
                if (data.Translations.TryGetValue(key, out string value))
                    return value;

            return null;
        }
    }
}