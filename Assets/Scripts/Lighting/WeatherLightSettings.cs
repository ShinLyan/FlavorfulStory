using UnityEngine;

namespace FlavorfulStory.Lightning
{
    [CreateAssetMenu(fileName = "WeatherLightSettings", menuName = "FlavorfulStory/WeatherLightSettings")]
    public class WeatherLightSettings : ScriptableObject
    {
        public WeatherType WeatherType;
        public LightSettings LightSettings;
    }
}