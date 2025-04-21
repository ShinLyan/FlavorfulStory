using System;
using UnityEngine;
using UnityEngine.Events;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Представляет событие, связанное с диалогом. </summary>
    /// <remarks> Используется для привязки пользовательских действий к событиям внутри диалога,
    /// например, чтобы запускать анимации, квесты, перемещения NPC и другие игровые события,
    /// когда игрок или NPC выполняет определенное действие в диалоге. </remarks>
    [Serializable]
    public class DialogueEvent
    {
        /// <summary> Имя действия, с которым связано событие. </summary>
        /// <remarks> Это строка-сигнал, которая будет сравниваться с текущим действием диалоговой системы.
        /// Если в процессе диалога будет вызвано действие с таким же именем, то сработает <see cref="OnTrigger"/>.
        /// Примеры действий: "GiveItem", "OpenDoor", "StartQuest". </remarks>
        [field: Tooltip("Имя действия, которое запускает событие (например, 'GiveItem', 'OpenDoor')."), SerializeField]
        public string ActionName { get; private set; }

        /// <summary> Unity-событие, которое будет вызвано, когда действие совпадет с <see cref="ActionName"/>. </summary>
        /// <remarks> Может содержать любую логику, заданную в редакторе: Запуск анимаций, Передача предметов,
        /// Переключение состояний квестов, Включение UI и многое другое. Событие вызывается только если
        /// в процессе диалога произойдет вызов действия с именем <see cref="ActionName"/>. </remarks>
        [field: Tooltip("Событие, которое сработает при совпадении действия."), SerializeField]
        public UnityEvent OnTrigger { get; private set; }
    }
}