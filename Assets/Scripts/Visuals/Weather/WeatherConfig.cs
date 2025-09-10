using UnityEngine;

namespace FlavorfulStory.Visuals.Weather
{
    /// <summary> Конфигурация погодных условий в игре. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Visuals/WeatherConfig")]
    public class WeatherConfig : ScriptableObject
    {
        /// <summary> Массив настроек погодных условий. </summary>
        public WeatherSettings[] WeatherSettings;
    }
}