using UnityEngine;

namespace FlavorfulStory.Lightning
{
    /// <summary> Настройки освещения для конкретного типа погоды. </summary>
    /// <remarks> Создаваемый ассет, который можно создать через меню Unity. </remarks> 
    [CreateAssetMenu(fileName = "WeatherLightSettings", menuName = "FlavorfulStory/WeatherLightSettings")]
    public class WeatherLightSettings : ScriptableObject
    {
        /// <summary> Тип погоды, к которому применяются эти настройки освещения. </summary>
        public WeatherType WeatherType;

        /// <summary> Настройки освещения (солнце и луна) для данного типа погоды. </summary>
        public LightSettings LightSettings;
    }
}