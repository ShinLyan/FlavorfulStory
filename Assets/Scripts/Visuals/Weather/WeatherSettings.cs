using System;
using FlavorfulStory.Utils.Attributes;
using UnityEngine;

namespace FlavorfulStory.Visuals.Weather
{
    /// <summary> Настройки погодных условий. </summary>
    /// <remarks> Используется системой генерации погоды для создания разнообразных погодных условий. </remarks>
    [Serializable]
    public class WeatherSettings
    {
        /// <summary> Тип погодных условий. </summary>
        /// <remarks> Определяет, какой набор настроек освещения будет использоваться. </remarks>
        [field: Tooltip("Тип погодных условий."), SerializeField]
        public WeatherType WeatherType { get; private set; }

        /// <summary> Вероятность появления данного типа погоды (от 0 до 1). </summary>
        /// <remarks> Используется в алгоритме взвешенной случайной выборки для генерации погоды.
        /// Сумма всех вероятностей в массиве настроек должна быть равна 1.0. </remarks>
        [field: Tooltip("Вероятность появления данного типа погоды (от 0 до 1)."), SerializeField,
                SteppedRange(0f, 1f, 0.1f)]
        public float Probability { get; private set; }

        /// <summary> Объект с системой частиц для эффектов погоды. </summary>
        /// <remarks> Автоматически активируется при выборе соответствующего типа погоды.
        /// Может быть null для погодных условий без визуальных эффектов. </remarks>
        [field: Tooltip("Объект с системой частиц для эффектов погоды."), SerializeField]
        public ParticleSystem ParticleObject { get; private set; }
    }
}