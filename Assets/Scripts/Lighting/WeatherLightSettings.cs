using UnityEngine;

namespace FlavorfulStory.Lightning
{
    /// <summary> Настройки освещения для конкретного типа погоды. </summary>
    /// <remarks> Создаваемый ассет, который можно создать через меню Unity. </remarks> 
    [CreateAssetMenu(menuName = "FlavorfulStory/WeatherLightSettings")]
    public class WeatherLightSettings : ScriptableObject
    {
        /// <summary> Тип погоды, к которому применяются эти настройки освещения. </summary>
        [field: Tooltip("Тип погоды, к которому применяются эти настройки освещения."), SerializeField]
        public WeatherType WeatherType { get; private set; }

        /// <summary> Настройки освещения (солнце и луна) для данного типа погоды. </summary>
        [field: Tooltip("Настройки освещения (солнце и луна) для данного типа погоды."), SerializeField]
        public LightSettings LightSettings { get; private set; }
    }
}