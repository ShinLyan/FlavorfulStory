using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    [CreateAssetMenu(fileName = "TimeSettings", menuName = "FlavorfulStory/TimeSettings")]
    public class TimeSettings : ScriptableObject
    {
        public int SunriseHour = 6;
        public int SunsetHour = 20;
    }
}