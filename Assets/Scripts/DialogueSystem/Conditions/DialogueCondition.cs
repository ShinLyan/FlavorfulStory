using System;
using UnityEngine;

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
        /// <returns> Числовой вес условия.. </returns>
        public abstract int GetWeight();

        public virtual DialogueCondition Clone()
        {
            // Стандартная реализация через сериализацию JSON
            string json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson(json, GetType()) as DialogueCondition;
        }
    }
}