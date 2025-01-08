using System;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Дни недели.</summary>
    [Serializable]
    public enum WeekDays
    {
        Mon = 0,
        Tue = 1,
        Wed = 2,
        Thu = 3,
        Fri = 4,
        Sat = 5,
        Sun = 6
    }

    /// <summary> Сезоны.</summary>
    [Serializable]
    public enum Seasons
    {
        Winter = 0,
        Spring = 1,
        Summer = 2,
        Autumn = 3
    }
}