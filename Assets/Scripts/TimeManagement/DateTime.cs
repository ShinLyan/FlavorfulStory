using System;
using System.Collections.Generic;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет игровым временем, позволяя отслеживать годы, сезоны, дни и время суток. </summary>
    [Serializable]
    public struct DateTime
    {
        #region Fields and Properties

        /// <summary> Общее количество минут, прошедших с начала игры. </summary>
        private int _totalMinutes;

        /// <summary> Словарь для конвертации дней недели. </summary>
        private static readonly Dictionary<DayOfWeek, DayOfWeek> DayOfWeekConvertDict = new()
        {
            { (DayOfWeek)1, (DayOfWeek)1 },
            { (DayOfWeek)2, (DayOfWeek)2 },
            { (DayOfWeek)3, (DayOfWeek)4 },
            { (DayOfWeek)4, (DayOfWeek)8 },
            { (DayOfWeek)5, (DayOfWeek)16 },
            { (DayOfWeek)6, (DayOfWeek)32 },
            { (DayOfWeek)7, (DayOfWeek)64 }
        };

        /// <summary> Словарь для конвертации сезонов. </summary>
        private static readonly Dictionary<Season, Season> SeasonConvertDict = new()
        {
            { (Season)1, (Season)1 },
            { (Season)2, (Season)2 },
            { (Season)3, (Season)4 },
            { (Season)4, (Season)8 }
        };

        /// <summary> Количество дней в одном сезоне. </summary>
        private const int DaysCount = 28;

        /// <summary> Количество минут в одном дне. </summary>
        private const int DayMinutes = 60 * 24;

        /// <summary> Количество минут в одном сезоне. </summary>
        private const int SeasonMinutes = DayMinutes * DaysCount;

        /// <summary> Получает текущий год. </summary>
        public int Year => _totalMinutes / (SeasonMinutes * 4) + 1;

        /// <summary> Получает текущий сезон. </summary>
        public Season Season => SeasonConvertDict[(Season)(_totalMinutes % (SeasonMinutes * 4) / SeasonMinutes)];

        /// <summary> Получает день в текущем сезоне. </summary>
        public int SeasonDay => _totalMinutes % SeasonMinutes / DayMinutes + 1;

        /// <summary> Получает день недели. </summary>
        public DayOfWeek DayOfWeek => DayOfWeekConvertDict[(DayOfWeek)(_totalMinutes / DayMinutes % 7 + 1)];

        /// <summary> Получает текущий час. </summary>
        public int Hour => _totalMinutes % DayMinutes / 60;

        /// <summary> Получает текущую минуту. </summary>
        public int Minute => _totalMinutes % 60;

        /// <summary> Получает количество полных недель, прошедших с начала игры. </summary>
        public int TotalWeeks => _totalMinutes / (DayMinutes * 7);

        /// <summary> Получает количество полных дней, прошедших с начала игры. </summary>
        public int TotalDays => _totalMinutes / DayMinutes;

        #endregion

        /// <summary> Создаёт объект игровой даты и времени. </summary>
        /// <param name="year"> Год. </param>
        /// <param name="season"> Сезон. </param>
        /// <param name="day"> День в сезоне. </param>
        /// <param name="hour"> Часы. </param>
        /// <param name="minute"> Минуты. </param>
        public DateTime(int year, Season season, int day, int hour, int minute)
        {
            _totalMinutes = (year - 1) * SeasonMinutes * 4 // Годы в минутах
                            + (int)season * SeasonMinutes // Сезоны в минутах
                            + (day - 1) * DayMinutes // Дни в минутах
                            + hour * 60 // Часы в минутах
                            + minute; // Минуты
        }

        /// <summary> Создаёт объект времени, задав общее количество минут. </summary>
        /// <param name="totalMinutes"> Общее количество минут. </param>
        public DateTime(int totalMinutes) => _totalMinutes = totalMinutes;

        /// <summary> Добавляет указанное количество минут к текущему времени. </summary>
        /// <param name="minutes"> Количество минут для добавления. </param>
        public void AddMinutes(int minutes) => _totalMinutes += minutes;

        #region ToString

        /// <summary> Преобразует текущую дату и время в строку. </summary>
        /// <returns> Строковое представление текущей даты и времени. </returns>
        public override string ToString() => $"{Year} year {Season} {SeasonDay} day {TimeToString()}";

        /// <summary> Преобразует текущую дату в строку. </summary>
        /// <returns> Строковое представление текущей даты. </returns>
        public string DateToString() => $"{DayOfWeek} {SeasonDay} {Year}";

        /// <summary> Преобразует время в строку с учётом выбранного формата отображения. </summary>
        /// <param name="is24HourFormat"> Если true, время отображается в 24-часовом формате (например, "14:05"). 
        /// Если false, время отображается в 12-часовом формате с AM/PM (например, "02:05 PM"). </param>
        /// <returns> Строковое представление времени. </returns> 
        public string TimeToString(bool is24HourFormat = true)
        {
            if (is24HourFormat)
            {
                // 24-часовой формат
                var hourString = Hour.ToString("D2");
                var minuteString = Minute.ToString("D2");
                return $"{hourString}:{minuteString}";
            }
            else
            {
                // 12-часовой AM/PM формат
                var period = Hour >= 12 ? "PM" : "AM";
                var hour12 = Hour % 12;
                if (hour12 == 0) hour12 = 12; // Если час = 0 или 12, то в 12-часовом формате это будет 12
                var hourString = hour12.ToString("D2");
                var minuteString = Minute.ToString("D2");
                return $"{hourString}:{minuteString} {period}";
            }
        }

        #endregion
    }
}