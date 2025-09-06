using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Conditions
{
    /// <summary> Пара диалога и условий его показа. </summary>
    [Serializable]
    public class ConditionalDialogue
    {
        /// <summary> Диалог для показа. </summary>
        [Tooltip("Диалог, который будет показан, если выполнены условия.")]
        public Dialogue Dialogue;

        /// <summary> Условия показа диалога. </summary>
        [Tooltip("Условия, при которых доступен этот диалог."), SerializeReference]
        public List<DialogueCondition> Conditions;
    }
}