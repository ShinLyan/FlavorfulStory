using System.Collections.Generic;
using FlavorfulStory.DialogueSystem;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> Поисковик использования ключей локализации в диалогах. </summary>
    public class DialogueLocalizationUsageFinder : ILocalizationUsageFinder
    {
        /// <summary> Массив всех диалогов в проекте. </summary>
        private Dialogue[] _allDialogues;

        /// <summary> Инициализирует новый экземпляр поисковика и загружает диалоги. </summary>
        public DialogueLocalizationUsageFinder() => LoadDialogues();

        /// <summary> Загружает все диалоговые ресурсы из папки Resources. </summary>
        private void LoadDialogues() => _allDialogues = Resources.LoadAll<Dialogue>("");

        /// <summary> Находит все использования ключа локализации в диалогах. </summary>
        /// <param name="key"> Ключ локализации для поиска (без квадратных скобок). </param>
        /// <returns> Список узлов диалога, содержащих указанный ключ. </returns>
        public List<Object> FindUsages(string key)
        {
            var results = new List<Object>();
            if (_allDialogues == null || _allDialogues.Length == 0) LoadDialogues();

            foreach (var dialogue in _allDialogues)
            {
                if (!dialogue) continue;

                foreach (var node in dialogue.Nodes)
                    if (node && node.Text.Contains($"[{key}]"))
                        results.Add(node);
            }

            return results;
        }
    }
}