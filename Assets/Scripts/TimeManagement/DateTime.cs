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
        private float _totalMinutes;

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
            { (Season)1, (Season)1 }, { (Season)2, (Season)2 }, { (Season)3, (Season)4 }, { (Season)4, (Season)8 }
        };

        /// <summary> Количество дней в одном сезоне. </summary>
        private const int DaysCount = 28;

        /// <summary> Количество минут в одном дне. </summary>
        private const float DayMinutes = 60f * 24f;

        /// <summary> Количество минут в одном сезоне. </summary>
        private const float SeasonMinutes = DayMinutes * DaysCount;

        /// <summary> Получает текущий год. </summary>
        public float Year => _totalMinutes / (SeasonMinutes * 4) + 1;

        /// <summary> Получает текущий сезон. </summary>
        public Season Season => SeasonConvertDict[(Season)(TotalDays % (DaysCount * 4) / DaysCount)];

        /// <summary> Получает день в текущем сезоне. </summary>
        public float SeasonDay
        {
            get
            {
                float adjustedTotalMinutes = _totalMinutes - 2 * 60;
                if (adjustedTotalMinutes < 0) return 1;
                return adjustedTotalMinutes % SeasonMinutes / DayMinutes + 1;
            }
        }

        /// <summary> Получает день недели. </summary>
        public DayOfWeek DayOfWeek
        {
            get
            {
                float adjustedTotalMinutes = _totalMinutes - 2 * 60;
                if (adjustedTotalMinutes < 0) adjustedTotalMinutes += 7 * DayMinutes; 
                int dayOfWeekIndex = (int)(adjustedTotalMinutes / DayMinutes % 7 + 1);
                return DayOfWeekConvertDict[(DayOfWeek)dayOfWeekIndex];
            }
        }

        /// <summary> Получает текущий час. </summary>
        public float Hour => _totalMinutes % DayMinutes / 60;

        /// <summary> Получает текущую минуту. </summary>
        public float Minute => _totalMinutes % 60;

        /// <summary> Получает количество полных недель, прошедших с начала игры. </summary>
        public int TotalWeeks => (int)(_totalMinutes / (DayMinutes * 7));

        /// <summary> Получает количество полных дней, прошедших с начала игры. </summary>
        public int TotalDays => (int)(_totalMinutes / DayMinutes);

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

        /// <summary> Создаёт объект игровой даты и времени. </summary>
        /// <param name="year"> Год. </param>
        /// <param name="season"> Сезон. </param>
        /// <param name="day"> День в сезоне. </param>
        /// <param name="hour"> Часы. </param>
        /// <param name="minute"> Минуты. </param>
        public DateTime(float year, Season season, float day, float hour, float minute)
        {
            _totalMinutes = (int)(year - 1) * SeasonMinutes * 4 // Годы в минутах
                            + (int)season * SeasonMinutes // Сезоны в минутах
                            + (int)(day - 1) * DayMinutes // Дни в минутах
                            + (int)hour * 60 // Часы в минутах
                            + (int)minute; // Минуты
        }

        /// <summary> Создаёт объект времени, задав общее количество минут. </summary>
        /// <param name="totalMinutes"> Общее количество минут. </param>
        public DateTime(int totalMinutes) => _totalMinutes = totalMinutes;

        /// <summary> Добавляет указанное количество минут к текущему времени. </summary>
        /// <param name="minutes"> Количество минут для добавления. </param>
        /// <returns> Новый объект DateTime с добавленными минутами. </returns>
        public DateTime AddMinutes(float minutes)
        {
            var newTime = this;
            newTime._totalMinutes += minutes;
            return newTime;
        }

        #region ToString

        /// <summary> Преобразует текущую дату и время в строку. </summary>
        /// <returns> Строковое представление текущей даты и времени. </returns>
        public override string ToString() => $"{Year} year {Season} {SeasonDay} day {TimeToString()}";

        /// <summary> Преобразует текущую дату в строку. </summary>
        /// <returns> Строковое представление текущей даты. </returns>
        public string DateToString() =>
            $"Year: {Year}, Season: {Season.ToString()}, Day of week: {DayOfWeek.ToString()}, Day of month: {SeasonDay}";

        /// <summary> Преобразует время в строку с учётом выбранного формата отображения. </summary>
        /// <param name="is24HourFormat"> Если true, время отображается в 24-часовом формате (например, "14:05"). 
        /// Если false, время отображается в 12-часовом формате с AM/PM (например, "02:05 PM"). </param>
        /// <returns> Строковое представление времени. </returns> 
        public string TimeToString(bool is24HourFormat = true)
        {
            if (is24HourFormat)
            {
                // 24-часовой формат
                string hourString = ((int)Hour).ToString("D2");
                string minuteString = ((int)Minute).ToString("D2");
                return $"{hourString}:{minuteString}";
            }
            else
            {
                // 12-часовой AM/PM формат
                string period = Hour >= 12 ? "PM" : "AM";
                int hour12 = (int)Hour % 12;
                if (hour12 == 0) hour12 = 12; // Если час = 0 или 12, то в 12-часовом формате это будет 12
                string hourString = hour12.ToString("D2");
                string minuteString = ((int)Minute).ToString("D2");
                return $"{hourString}:{minuteString} {period}";
            }
        }

        #endregion
    }
}