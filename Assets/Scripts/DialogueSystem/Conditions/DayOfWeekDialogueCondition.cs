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
        public DayOfWeek DayOfWeek { get; private set; } = DayOfWeek.Any;

        /// <summary> Проверяет, соответствует ли текущий день недели условию. </summary>
        /// <returns> True, если текущий день соответствует условию. </returns>
        public override bool MatchesCurrentState()
        {
            var currentDay = WorldTime.CurrentGameTime.DayOfWeek;
            return DayOfWeek == DayOfWeek.Any || DayOfWeek == currentDay;
        }

        /// <summary> Получает вес условия из конфигурации. </summary>
        /// <param name="config"> Конфигурация весов диалогов. </param>
        /// <returns> Вес условия. </returns>
        public override int GetWeight(DialogueWeightsConfig config) =>
            DayOfWeek != DayOfWeek.Any ? config.DayOfWeekWeight : 0;
    }
}