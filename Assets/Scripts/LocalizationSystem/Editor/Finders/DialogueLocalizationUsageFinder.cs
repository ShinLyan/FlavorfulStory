using System.Collections.Generic;
using FlavorfulStory.DialogueSystem;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public class DialogueLocalizationUsageFinder : ILocalizationUsageFinder
    {
        private Dialogue[] _allDialogues;

        public DialogueLocalizationUsageFinder() { LoadDialogues(); }

        private void LoadDialogues() { _allDialogues = Resources.LoadAll<Dialogue>(""); }

        public List<Object> FindUsages(string key)
        {
            var results = new List<Object>();
            if (_allDialogues == null || _allDialogues.Length == 0) LoadDialogues();

            foreach (var dialogue in _allDialogues)
            {
                if (dialogue == null) continue;

                foreach (var node in dialogue.Nodes)
                    if (node != null && node.Text.Contains($"[{key}]"))
                        results.Add(node);
            }

            return results;
        }
    }
}