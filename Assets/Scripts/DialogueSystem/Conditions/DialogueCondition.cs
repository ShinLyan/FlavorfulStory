using System;

namespace FlavorfulStory.DialogueSystem.Conditions
{
    /// <summary> Базовый класс для условий диалога. </summary>
    [Serializable]
    public abstract class DialogueCondition
    {
        /// <summary> Проверяет соответствие текущему состоянию. </summary>
        /// <returns> True, если условие выполнено. </returns>
        public abstract bool MatchesCurrentState();

        /// <summary> Получает вес условия из конфига. </summary>
        /// <param name="config"> Конфигурация весов. </param>
        /// <returns> Числовой вес условия.. </returns>
        public abstract int GetWeight(DialogueWeightsConfig config);
    }
}