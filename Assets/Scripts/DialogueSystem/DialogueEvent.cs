using System;
using UnityEngine;
using UnityEngine.Events;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Событыие диалога. </summary>
    [Serializable]
    public class DialogueEvent
    {
        /// <summary> Название действия, которое вызывает событие. </summary>
        [field: SerializeField]
        public string ActionName { get; private set; }

        /// <summary> Событие, которое будет вызвано при совпадении действия. </summary>
        [field: SerializeField]
        public UnityEvent OnTriggered { get; private set; }
    }
}