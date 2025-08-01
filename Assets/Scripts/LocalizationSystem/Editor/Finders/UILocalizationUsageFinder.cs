#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> Поисковик использования ключей локализации в UI элементах. </summary>
    public class UILocalizationUsageFinder : ILocalizationUsageFinder
    {
        /// <summary> Находит все UI элементы, использующие указанный ключ локализации. </summary>
        /// <param name="key"> Ключ локализации для поиска (без учета регистра). </param>
        /// <returns> Список GameObject с компонентами LocalizedLabel, использующими указанный ключ. </returns>
        public List<Object> FindUsages(string key)
        {
            var results = new List<Object>();
            if (string.IsNullOrWhiteSpace(key)) return results;

            var labels =
                Object.FindObjectsByType<LocalizedLabel>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var label in labels)
            {
                var so = new SerializedObject(label);
                var prop = so.FindProperty("_localizationKey");
                if (prop == null) continue;

                if (string.Equals(prop.stringValue, key, StringComparison.OrdinalIgnoreCase))
                    if (!results.Contains(label.gameObject))
                        results.Add(label.gameObject);
            }

            return results;
        }
    }
}
#endif