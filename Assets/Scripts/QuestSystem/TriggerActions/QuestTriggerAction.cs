using System;

namespace FlavorfulStory.QuestSystem.TriggerActions
{
    /// <summary> Базовый класс действия, выполняемого после завершения этапа квеста. </summary>
    [Serializable]
    public abstract class QuestTriggerAction
    {
        /// <summary> Выполняет действие в контексте квеста. </summary>
        /// <param name="context"> Контекст выполнения квеста. </param>
        public abstract void Execute(QuestExecutionContext context);
    }
}