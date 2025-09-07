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
        /// <summary> Диалог, который может быть показан при выполнении всех условий. </summary>
        [field: Tooltip("Диалог, который может быть показан при выполнении всех условий."), SerializeField]
        public Dialogue Dialogue { get; private set; }

        /// <summary> Условия, при которых диалог становится доступным. </summary>
        [field: Tooltip("Условия, при которых диалог становится доступным."), SerializeReference]
        public List<DialogueCondition> Conditions { get; private set; }
    }
}