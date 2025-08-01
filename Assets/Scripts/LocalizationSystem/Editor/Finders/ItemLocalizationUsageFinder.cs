using System.Collections.Generic;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public class ItemLocalizationUsageFinder : ILocalizationUsageFinder
    {
        private InventoryItem[] _cachedItems;

        private void LoadItems() { _cachedItems = Resources.LoadAll<InventoryItem>(""); }

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