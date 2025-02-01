using System;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Перечисление дней недели. </summary>
    [Serializable]
    public enum WeekDays
    {
        /// <summary> Понедельник. </summary>
        Monday,

        /// <summary> Вторник. </summary>
        Tuesday,

        /// <summary> Среда. </summary>
        Wednesday,

        /// <summary> Четверг. </summary>
        Thursday,

        /// <summary> Пятница. </summary>
        Friday,

        /// <summary> Суббота. </summary>
        Saturday,

        /// <summary> Воскресенье. </summary>
        Sunday
    }

    /// <summary> Перечисление сезонов. </summary>
    [Serializable]
    public enum Seasons
    {
        /// <summary> Весна. </summary>
        Spring,

        /// <summary> Лето. </summary>
        Summer,

        /// <summary> Осень. </summary>
        Autumn,

        /// <summary> Зима. </summary>
        Winter
    }
}