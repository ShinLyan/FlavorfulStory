using System.Collections.Generic;
using FlavorfulStory.AI;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Selectors
{
    /// <summary> Селектор для приветственных диалогов NPC. </summary>
    public class GreetingDialogueSelector : IDialogueSelector
    {
        /// <summary> Словарь для отслеживания приветствованных NPC. </summary>
        private readonly Dictionary<NpcName, bool> _greetedNpcs = new();

        /// <summary> Выбирает приветственный диалог для NPC. </summary>
        /// <param name="npcName"> Имя NPC. </param>
        /// <returns> Приветственный диалог или null. </returns>
        public Dialogue SelectDialogue(NpcName npcName)
        {
            if (_greetedNpcs.TryGetValue(npcName, out bool greeted) && greeted) return null;

            var dialogues = DialogueDatabase.GetDialoguesFromNameAndType(npcName, DialogueType.Greeting);

            if (dialogues.Count == 0)
            {
                Debug.LogError($"Для NPC {npcName} не существует приветственного диалога!");
                return null;
            }

            _greetedNpcs[npcName] = true;

            return dialogues[0];
        }
    }
}