using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Конфигурация весов для условий диалогов. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/DialogueSystem/WeightsConfig")]
    public class DialogueWeightsConfig : ScriptableObject
    {
        /// <summary> Вес для условий, связанных с погодой. </summary>
        public int WeatherWeight = 10;

        /// <summary> Вес для условий, связанных с временем суток. </summary>
        public int TimeOfDayWeight = 20;

        /// <summary> Вес для условий, связанных с днём недели. </summary>
        public int DayOfWeekWeight = 50;
    }
}