using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> Менеджер для поиска использования ключей локализации в проекте. </summary>
    public class LocalizationUsageFinderManager
    {
        /// <summary> Словарь поисковиков использования по группам ключей. </summary>
        private readonly Dictionary<string, ILocalizationUsageFinder> _finders = new();

        /// <summary> Инициализирует менеджер, регистрируя поисковики для различных групп. </summary>
        public LocalizationUsageFinderManager()
        {
            _finders["dialogue"] = new DialogueLocalizationUsageFinder();
            _finders["quest"] = new QuestLocalizationUsageFinder();
            _finders["item"] = new ItemLocalizationUsageFinder();
            _finders["ui"] = new UILocalizationUsageFinder();
        }

        /// <summary> Находит все использования ключа локализации в проекте. </summary>
        /// <param name="group"> Группа ключа. </param>
        /// <param name="key"> Ключ локализации для поиска. </param>
        /// <returns> Список объектов, использующих указанный ключ локализации. </returns>
        public List<Object> FindUsages(string group, string key)
        {
            string lowerGroup = group.ToLower();
            if (_finders.TryGetValue(lowerGroup, out var finder)) return finder.FindUsages(key);

            return new List<Object>();
        }
    }
}