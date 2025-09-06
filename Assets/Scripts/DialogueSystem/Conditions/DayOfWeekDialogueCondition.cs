using System;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DayOfWeek = FlavorfulStory.TimeManagement.DayOfWeek;

namespace FlavorfulStory.DialogueSystem.Conditions
{
    /// <summary> Условие диалога, проверяющее день недели. </summary>
    [Serializable]
    public class DayOfWeekDialogueCondition : DialogueCondition
    {
        /// <summary> Требуемый день недели для выполнения условия. </summary>
        [field: SerializeField]
        public DayOfWeek DayOfWeek { get; private set; }

        /// <summary> Проверяет, соответствует ли текущий день недели условию. </summary>
        /// <returns> True, если текущий день соответствует условию. </returns>
        public override bool MatchesCurrentState() => DayOfWeek == WorldTime.CurrentDateTime.DayOfWeek;

        /// <summary> Получает вес условия из конфигурации. </summary>
        /// <returns> Вес условия. </returns>
        public override int GetWeight() => DialogueWeightsConfig.DayOfWeekWeight;
    }
}