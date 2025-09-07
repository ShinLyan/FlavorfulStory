using System;

namespace FlavorfulStory.DialogueSystem.Conditions
{
    /// <summary> Базовый класс для условий диалога. </summary>
    [Serializable]
    public abstract class DialogueCondition
    {
        /// <summary> Возвращает true, если условие выполнено. </summary>
        public abstract bool IsSatisfied { get; }

        /// <summary> Вес условия, используется для приоритизации. </summary>
        public abstract int Weight { get; }
    }
}