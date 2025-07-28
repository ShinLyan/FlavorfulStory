using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary>
    /// Компонент, который обновляет все TMP_Text с ключами локализации в формате {{KEY}}.
    /// </summary>
    public class UILocalizer : MonoBehaviour
    {
        private static readonly List<UILocalizer> _allInstances = new();

        private void Awake() => _allInstances.Add(this);
        private void OnDestroy() => _allInstances.Remove(this);

        private void Start() => LocalizeAllTextElements();

        public static void RefreshAll()
        {
            foreach (var instance in _allInstances) instance.LocalizeAllTextElements();
        }

        public void LocalizeAllTextElements()
        {
            var texts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in texts)
            {
                if (t == null || string.IsNullOrWhiteSpace(t.text)) continue;

                if (t.text.StartsWith("{{") && t.text.EndsWith("}}"))
                {
                    string key = t.text.Trim('{', '}');
                    t.text = LocalizationService.Get(key);
                }
            }
        }
    }
}