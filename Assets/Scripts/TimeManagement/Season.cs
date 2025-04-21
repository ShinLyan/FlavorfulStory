using System;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Перечисление сезонов. </summary>
    [Serializable, Flags]
    public enum Season
    {
        /// <summary> Весна. </summary>
        Spring = 1 << 0,

        /// <summary> Лето. </summary>
        Summer = 1 << 1,

        /// <summary> Осень. </summary>
        Autumn = 1 << 2,

        /// <summary> Зима. </summary>
        Winter = 1 << 3
    }
}