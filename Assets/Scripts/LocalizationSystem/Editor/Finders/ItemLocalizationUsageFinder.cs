using System.Collections.Generic;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> Поисковик использования ключей локализации в предметах инвентаря. </summary>
    public class ItemLocalizationUsageFinder : ILocalizationUsageFinder
    {
        /// <summary> Кэшированный массив всех предметов инвентаря. </summary>
        private InventoryItem[] _cachedItems;

        /// <summary> Загружает все предметы из папки Resources. </summary>
        private void LoadItems() => _cachedItems = Resources.LoadAll<InventoryItem>("");

        /// <summary> Находит предметы, использующие указанный ключ локализации. </summary>
        /// <param name="key"> Ключ локализации для поиска. </param>
        /// <returns> Список предметов, использующих ключ в названии или описании. </returns>
        public List<Object> FindUsages(string key)
        {
            if (_cachedItems == null || _cachedItems.Length == 0) LoadItems();

            var results = new List<Object>();

            foreach (var item in _cachedItems)
            {
                if (item == null) continue;

                bool nameMatch = item.ItemNameKey != null && item.ItemNameKey.Contains($"[{key}]");
                bool descMatch = item.DescriptionKey != null && item.DescriptionKey.Contains($"[{key}]");

                if (nameMatch || descMatch) results.Add(item);
            }

            return results;
        }
    }
}