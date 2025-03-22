using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> База данных всех предметов. </summary>
    public static class ItemDatabase
    {
        /// <summary> Словарь предметов. </summary>
        /// <remarks> Хранит пары (GUID, предмет). </remarks>
        private static Dictionary<string, InventoryItem> _itemDatabase;

        /// <summary> Создать базу данных предметов. </summary>
        public static void Initialize()
        {
            if (_itemDatabase != null) return;

            _itemDatabase = new Dictionary<string, InventoryItem>();

            // Загрузка все ресурсов с типом InventoryItem по всему проекту.
            var items = Resources.LoadAll<InventoryItem>(string.Empty);
            foreach (var item in items)
            {
                if (_itemDatabase.TryGetValue(item.ItemID, out var value))
                {
                    Debug.LogError("Присутствует дубликат ID InventoryItem для объектов: " +
                                   $"{value} и {item}. Замените ID у данного объекта.");
                    continue;
                }

                _itemDatabase[item.ItemID] = item;
            }
        }

        /// <summary> Получить экземпляр предмета инвентаря по ID. </summary>
        /// <param name="itemID"> ID предмета инвентаря. </param>
        /// <returns> Экземпляр <see cref="InventoryItem"/>, соответствующий ID. </returns>
        public static InventoryItem GetItemFromID(string itemID)
        {
            if (itemID == null || !_itemDatabase.TryGetValue(itemID, out var item)) return null;
            return item;
        }
    }
}