using System;
using System.Collections.Generic;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> Сервис для работы с локализацией, предоставляющий методы получения переводов и управления языками. </summary>
    public class LocalizationService
    {
        /// <summary> Словарь всех переводов с группировкой по ключам локализации. </summary>
        private readonly Dictionary<string, Dictionary<string, string>> _translationsPerKey;

        /// <summary> Доступные языки локализации. </summary>
        private readonly HashSet<string> _languages;

        /// <summary> Текущий словарь переводов для выбранного языка. </summary>
        private Dictionary<string, string> _currentMap;

        /// <summary> Текущий установленный язык локализации. </summary>
        private string _currentLang;

        /// <summary> Статический экземпляр сервиса локализации. </summary>
        private static LocalizationService _instance;

        /// <summary> Проверяет, был ли инициализирован сервис локализации. </summary>
        public static bool IsInitialized => _instance != null;

        /// <summary> Текущий язык. </summary>
        public static string CurrentLanguage => _instance?._currentLang;

        /// <summary> Событие, вызываемое при изменении языка. </summary>
        public static event Action OnLanguageChanged;

        /// <summary> Создает новый экземпляр сервиса локализации. </summary>
        /// <param name="database"> База данных с переводами. </param>
        /// <param name="defaultLang"> Язык по умолчанию (по умолчанию "EN"). </param>
        public LocalizationService(LocalizationDatabase database, string defaultLang = "EN")
        {
            _translationsPerKey = database.ToDictionary();
            _languages = new HashSet<string>();

            foreach (var entry in _translationsPerKey.Values)
            foreach (string lang in entry.Keys)
                _languages.Add(lang);

            SetLanguage(defaultLang);
            _instance = this;
        }

        /// <summary> Устанавливает текущий язык локализации. </summary>
        /// <param name="lang"> Код языка для установки. </param>
        private void SetLanguage(string lang)
        {
            _currentLang = _languages.Contains(lang) ? lang : "EN";
            _currentMap = new Dictionary<string, string>();

            foreach (var kvp in _translationsPerKey)
                if (kvp.Value.TryGetValue(_currentLang, out string val))
                    _currentMap[kvp.Key] = val;

            OnLanguageChanged?.Invoke();
        }

        /// <summary> Получает локализованную строку по ключу. </summary>
        /// <param name="key">Ключ перевода.</param>
        /// <returns>Локализованная строка или пустая строка, если перевод не найден.</returns>
        private string Get(string key) => _currentMap.GetValueOrDefault(key, "");

        /// <summary> Статический метод для установки языка локализации. </summary>
        /// <param name="lang">Код языка для установки.</param>
        public static void SetLanguageStatic(string lang) => _instance?.SetLanguage(lang);

        /// <summary> Статический метод для получения локализованной строки. </summary>
        /// <param name="key">Ключ перевода (автоматически обрезает квадратные скобки если есть).</param>
        /// <returns>Локализованная строка или пустая строка, если перевод не найден.</returns>
        public static string GetLocalizedString(string key) => _instance?.Get(key.TrimStart('[').TrimEnd(']')) ?? "";

        /// <summary> Получает список всех доступных языков. </summary>
        /// <returns>Коллекция кодов доступных языков.</returns>
        public static IEnumerable<string> GetAllLanguages() => _instance?._languages;
    }
}