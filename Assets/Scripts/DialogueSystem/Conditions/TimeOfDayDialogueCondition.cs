using System;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Conditions
{
    /// <summary> Условие диалога, проверяющее время суток. </summary>
    [Serializable]
    public class TimeOfDayDialogueCondition : DialogueCondition
    {
        /// <summary> Требуемое время суток для выполнения условия. </summary>
        [field: SerializeField] public TimeOfDay TimeOfDay { get; private set; }

        /// <summary> Проверяет соответствие текущего времени суток условию. </summary>
        /// <returns> True, если текущее время соответствует условию. </returns>
        public override bool MatchesCurrentState()
        {
            var currentTime = WorldTime.CurrentDateTime.Hour < 17 ? TimeOfDay.Before17 : TimeOfDay.After17;
            return TimeOfDay == currentTime;
        }

        /// <summary> Получает вес условия. </summary>
        /// <returns> Вес условия. </returns>
        public override int GetWeight() => DialogueWeightsConfig.TimeOfDayWeight;
    }
}