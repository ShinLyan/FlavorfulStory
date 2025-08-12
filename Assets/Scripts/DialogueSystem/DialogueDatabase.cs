using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> База данных всех диалогов в игре. </summary>
    public static class DialogueDatabase
    {
        /// <summary> Словарь диалогов, сгруппированных по имени NPC. </summary>
        private static readonly Dictionary<NpcName, List<Dialogue>> _dialoguesByNpc;

        /// <summary> Статический конструктор для инициализации базы данных. </summary>
        static DialogueDatabase()
        {
            _dialoguesByNpc = new Dictionary<NpcName, List<Dialogue>>();

            var allDialogues = Resources.LoadAll<Dialogue>(string.Empty);

            foreach (var dialogue in allDialogues)
            {
                if (!_dialoguesByNpc.TryGetValue(dialogue.NpcName, out var list))
                {
                    list = new List<Dialogue>();
                    _dialoguesByNpc[dialogue.NpcName] = list;
                }

                list.Add(dialogue);
            }
        }

        /// <summary> Получает все диалоги указанного типа для данного NPC. </summary>
        /// <param name="npcName"> Имя NPC для поиска диалогов. </param>
        /// <param name="dialogueType"> Тип диалогов. </param>
        /// <returns> Список диалогов или пустой список, если диалогов не найдено. </returns>
        public static List<Dialogue> GetDialoguesFromNameAndType(NpcName npcName, DialogueType dialogueType)
        {
            if (_dialoguesByNpc.TryGetValue(npcName, out var dialogues))
                return dialogues.Where(d => d.DialogueType == dialogueType).ToList();

            return new List<Dialogue>();
        }
    }
}