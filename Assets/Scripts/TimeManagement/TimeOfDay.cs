namespace FlavorfulStory.TimeManagement
{
    /// <summary> Время суток для условий в игре. </summary>
    public enum TimeOfDay
    {
        /// <summary> Временной период до 17:00. </summary>
        Before17,

        /// <summary> Временной период после 17:00. </summary>
        After17,

        /// <summary> Любое время (используется когда время не имеет значения). </summary>
        Any
    }
}