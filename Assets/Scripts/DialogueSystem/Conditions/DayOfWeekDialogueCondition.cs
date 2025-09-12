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
        /// <summary> День недели, требуемый для выполнения условия. </summary>
        [field: Tooltip("День недели, требуемый для выполнения условия."), SerializeField]
        public DayOfWeek DayOfWeek { get; private set; }

        /// <summary> Проверяет, соответствует ли текущий день недели условию. </summary>
        public override bool IsSatisfied => DayOfWeek == WorldTime.CurrentGameTime.DayOfWeek;

        /// <summary> Вес условия. </summary>
        public override int Weight => DialogueWeightsConfig.DayOfWeekWeight;
    }
}