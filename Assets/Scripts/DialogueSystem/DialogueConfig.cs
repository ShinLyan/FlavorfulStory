using System.Collections.Generic;
using FlavorfulStory.DialogueSystem.Conditions;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Конфигурация диалогов для NPC. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/DialogueSystem/DialogueConfig")]
    public class DialogueConfig : ScriptableObject
    {
        /// <summary> Приветственный диалог. </summary>
        [field: Tooltip("Приветственный диалог."), SerializeField]
        public Dialogue GreetingDialogue { get; private set; }

        /// <summary> Список условных диалогов. </summary>
        [field: Tooltip("Диалоги с условиями."), SerializeField]
        public List<ConditionalDialogue> ConditionalDialogues { get; private set; }
    }
}