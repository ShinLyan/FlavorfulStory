using System;
using System.Collections.Generic;
using FlavorfulStory.DialogueSystem.Conditions;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Контекстный (повседневный) диалог. </summary>
    [Serializable]
    public struct ContextDialogue
    {
        /// <summary> Диалоги, которые могут быть показаны при выполнении всех условий. </summary>
        [field: Tooltip("Диалоги, которые могут быть показаны при выполнении всех условий."), SerializeField]
        public List<Dialogue> Dialogues { get; private set; }

        /// <summary> Условия, при которых диалог становится доступным. </summary>
        [field: Tooltip("Условия, при которых диалог становится доступным."), SerializeReference]
        public List<DialogueCondition> Conditions { get; private set; }
    }
}