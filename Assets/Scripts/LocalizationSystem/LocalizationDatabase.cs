using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> ScriptableObject, представляющий базу данных локализации с переводами. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Localization/Localization Database")]
    public class LocalizationDatabase : ScriptableObject
    {
        /// <summary> Список записей локализации с ключами и их переводами. </summary>
        [SerializeField] private List<LocalizationEntry> _entries = new();

        /// <summary> Преобразует записи базы данных в словарь. </summary>
        /// <returns> Словарь с переводами. </returns>
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

        /// <summary> Заменяет текущие записи новым списком записей локализации. </summary>
        /// <param name="entries">Новый список объектов LocalizationEntry.</param>
        public void SetEntries(List<LocalizationEntry> entries) => _entries = entries;

        /// <summary> Получает список всех уникальных кодов языков в базе данных. </summary>
        /// <returns> Список кодов языков в виде строк. </returns>
        public List<string> GetLanguages()
        {
            var languages = new HashSet<string>();
            foreach (var entry in _entries)
            foreach (var translation in entry.Translations)
                languages.Add(translation.LanguageCode);

            return new List<string>(languages);
        }
    }

    /// <summary> Представляет одну запись локализации с ключом и его переводами. </summary>
    [Serializable]
    public class LocalizationEntry
    {
        /// <summary> Ключ, используемый для идентификации этой записи локализации. </summary>
        public string Key;

        /// <summary> Список переводов для этой записи на разных языках. </summary>
        public List<LocalizedValue> Translations = new();
    }

    /// <summary> Представляет один перевод для конкретного языка. </summary>
    [Serializable]
    public class LocalizedValue
    {
        /// <summary> Идентификатор кода языка (например, "en", "ru", "es"). </summary>
        public string LanguageCode;

        /// <summary> Переведенный текст для этого языка. </summary>
        public string Text;
    }
}