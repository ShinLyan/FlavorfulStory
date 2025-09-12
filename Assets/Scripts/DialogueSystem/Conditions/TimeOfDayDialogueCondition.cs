using System;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Conditions
{
    /// <summary> Условие диалога, проверяющее время суток. </summary>
    [Serializable]
    public class TimeOfDayDialogueCondition : DialogueCondition
    {
        /// <summary> Время суток, требуемое для выполнения условия. </summary>
        [field: Tooltip("Время суток, требуемое для выполнения условия."), SerializeField]
        public TimeOfDay TimeOfDay { get; private set; }

        /// <summary> Проверяет соответствие текущего времени суток условию. </summary>
        public override bool IsSatisfied
        {
            get
            {
                var currentTime = WorldTime.CurrentGameTime.Hour < 17 ? TimeOfDay.Before17 : TimeOfDay.After17;
                return TimeOfDay == currentTime;
            }
        }

        /// <summary> Вес условия. </summary>
        public override int Weight => DialogueWeightsConfig.TimeOfDayWeight;
    }
}