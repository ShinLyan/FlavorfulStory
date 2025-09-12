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
        /// <param name="npcInfo"> Информация об NPC, для которого выбирается диалог. </param>
        /// <returns> Приветственный диалог или null, если уже приветствован или не задан. </returns>
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

        /// <summary> Сохраняет состояние — список NPC, которых уже приветствовали. </summary>
        /// <returns> Объект состояния (список имён NPC). </returns>
        public object CaptureState() => new List<NpcName>(_greetedNpcs.Keys);

        /// <summary> Восстанавливает состояние после загрузки. </summary>
        /// <param name="state"> Объект состояния, ранее сохранённый через <see cref="CaptureState"/>. </param>
        public void RestoreState(object state)
        {
            if (state is not List<NpcName> records) return;

            _greetedNpcs.Clear();

            foreach (var npc in records) _greetedNpcs[npc] = true;
        }

        #endregion
    }
}