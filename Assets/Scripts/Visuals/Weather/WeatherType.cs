namespace FlavorfulStory.Visuals.Weather
{
    /// <summary> Типы погодных условий в игре. </summary>
    /// <remarks> Используется для определения текущей погоды и соответствующих настроек освещения.
    /// Каждый тип соответствует определенному состоянию окружающей среды. </remarks>
    public enum WeatherType
    {
        /// <summary> Ясная солнечная погода. </summary>
        Clear,

        /// <summary> Дождливая погода. </summary>
        Rainy,

        // Cloudy, // Облачно
        // Stormy, // Гроза
        // Snowy, // Снег
        // Foggy // Туман

        /// <summary> Любая погода (используется когда погода не имеет значения). </summary>
        Any
    }
}