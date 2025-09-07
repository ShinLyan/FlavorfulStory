using System.Collections.Generic;
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

        /// <summary> Контекстные (повседневные) диалоги. </summary>
        [field: Tooltip("Контекстные (повседневные) диалоги. "), SerializeField]
        public List<ContextDialogue> ContextDialogues { get; private set; }
    }
}