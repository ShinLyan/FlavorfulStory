namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Конфигурация весов для условий диалогов. </summary>
    public class DialogueWeightsConfig
    {
        /// <summary> Вес для условий, связанных с погодой. </summary>
        public const int WeatherWeight = 50;

        /// <summary> Вес для условий, связанных с временем суток. </summary>
        public const int TimeOfDayWeight = 30;

        /// <summary> Вес для условий, связанных с днём недели. </summary>
        public const int DayOfWeekWeight = 20;
    }
}