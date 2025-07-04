using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Компонент, вызывающий события при срабатывании одного из указанных действий в диалоге.
    /// Поддерживает несколько действий для одного NPC. </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        /// <summary> Список действий и соответствующих событий. </summary>
        [field: SerializeField]
        public List<DialogueEvent> DialogueEvents { get; private set; }

        /// <summary> Выполнить событие, если действие совпадает с одним из зарегистрированных. </summary>
        /// <param name="actionToTrigger"> Имя действия, пришедшее из узла диалога. </param>
        public void TriggerDialogue(string actionToTrigger)
        {
            if (string.IsNullOrEmpty(actionToTrigger)) return;

            foreach (var dialogueEvent in DialogueEvents
                         .Where(dialogueEvent => dialogueEvent.ActionName == actionToTrigger))
                dialogueEvent.OnTrigger?.Invoke();
        }
    }
}