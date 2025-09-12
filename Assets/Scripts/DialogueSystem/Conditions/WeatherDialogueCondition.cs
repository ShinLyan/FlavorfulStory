using System;
using FlavorfulStory.Visuals.Weather;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Conditions
{
    /// <summary> Условие диалога, проверяющее текущую погоду. </summary>
    [Serializable]
    public class WeatherDialogueCondition : DialogueCondition
    {
        /// <summary> Погода, требуемая для выполнения условия. </summary>
        [field: Tooltip("Погода, требуемая для выполнения условия."), SerializeField]
        public WeatherType Weather { get; private set; }

        /// <summary> Проверяет соответствие текущей погоды условию. </summary>
        public override bool IsSatisfied => Weather == WeatherService.CurrentWeatherType;

        /// <summary> Вес условия. </summary>
        public override int Weight => DialogueWeightsConfig.WeatherWeight;
    }
}