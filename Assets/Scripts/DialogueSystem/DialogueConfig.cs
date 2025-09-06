using System.Collections.Generic;
using FlavorfulStory.DialogueSystem.Conditions;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Конфигурация диалогов для NPC. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/DialogueSystem/DialogueConfig")]
    public class DialogueConfig : ScriptableObject
    {
        /// <summary> Список приветственных диалогов. </summary>
        [field: Tooltip("Приветственный диалог NPC."), SerializeField]
        public List<Dialogue> GreetingDialogues { get; private set; }

        /// <summary> Список условных диалогов. </summary>
        [field: Tooltip("Диалоги с условиями."), SerializeField]
        public List<ConditionalDialogue> ConditionalDialogues { get; private set; }
    }
}