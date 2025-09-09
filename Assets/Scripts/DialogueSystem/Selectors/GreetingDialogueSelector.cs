using System;
using System.Collections.Generic;
using FlavorfulStory.AI;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Selectors
{
    /// <summary> Селектор для приветственных диалогов NPC. </summary>
    public class GreetingDialogueSelector : IDialogueSelector, ISaveableService
    {
        /// <summary> Словарь для отслеживания приветствованных NPC. </summary>
        private readonly Dictionary<NpcName, bool> _greetedNpcs = new();

        /// <summary> Выбирает приветственный диалог для NPC. </summary>
        /// <returns> Приветственный диалог или null. </returns>
        public Dialogue SelectDialogue(NpcInfo npcInfo)
        {
            var npcName = npcInfo.NpcName;
            if (_greetedNpcs.TryGetValue(npcName, out bool greeted) && greeted) return null;

            var dialogue = npcInfo.DialogueConfig.GreetingDialogue;
            if (!dialogue)
            {
                Debug.LogError($"Для NPC {npcName} не существует приветственного диалога!");
                return null;
            }

            _greetedNpcs[npcName] = true;
            return dialogue;
        }

        #region ISaveableService

        [Serializable]
        private struct State
        {
            public List<NpcName> GreetedNpcs;
        }

        public object CaptureState() => new State { GreetedNpcs = new List<NpcName>(_greetedNpcs.Keys) };

        public void RestoreState(object restoredState)
        {
            if (restoredState is State state)
            {
                _greetedNpcs.Clear();
                foreach (var npc in state.GreetedNpcs) _greetedNpcs[npc] = true;
            }
        }

        #endregion
    }
}