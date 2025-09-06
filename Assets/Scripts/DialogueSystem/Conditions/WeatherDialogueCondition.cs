using System;
using FlavorfulStory.Visuals.Weather;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Conditions
{
    /// <summary> Условие диалога, проверяющее текущую погоду. </summary>
    [Serializable]
    public class WeatherDialogueCondition : DialogueCondition
    {
        /// <summary> Требуемый тип погоды для выполнения условия. </summary>
        [field: SerializeField]
        public WeatherType Weather { get; private set; }

        /// <summary> Проверяет соответствие текущей погоды условию. </summary>
        /// <returns> True, если текущая погода соответствует условию или выбрано "Любая". </returns>
        public override bool MatchesCurrentState()
        {
            var currentWeather = WeatherType.Clear; //TODO: Заменить на Zenject Weather сервис
            return Weather == currentWeather;
        }

        /// <summary> Получает вес условия. </summary>
        /// <returns> Вес условия. </returns>
        public override int GetWeight() => DialogueWeightsConfig.WeatherWeight;
    }
}