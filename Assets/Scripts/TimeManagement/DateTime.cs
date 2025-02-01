using System;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет игровым временем, позволяя отслеживать годы, сезоны, дни и время суток. </summary>
    [Serializable]
    public struct DateTime
    {
        /// <summary> Общее количество минут, прошедших с начала игры. </summary>
        private int _totalMinutes;

        /// <summary> Количество дней в одном сезоне. </summary>
        private const int DaysCount = 28;

        /// <summary> Количество минут в одном дне. </summary>
        private const int DayMinutes = 60 * 24;

        /// <summary> Количество минут в одном сезоне. </summary>
        private const int SeasonMinutes = DayMinutes * DaysCount;

        #region Properties

        /// <summary> Получает текущий год. </summary>
        public int Year => _totalMinutes / (SeasonMinutes * 4) + 1;

        /// <summary> Получает текущий сезон. </summary>
        public Seasons Season => (Seasons)(_totalMinutes % (SeasonMinutes * 4) / SeasonMinutes);

        /// <summary> Получает день в текущем сезоне. </summary>
        public int DayInSeason => _totalMinutes % SeasonMinutes / DayMinutes + 1;

        /// <summary> Получает день недели. </summary>
        public WeekDays DayOfWeek => (WeekDays)(_totalMinutes / DayMinutes % 7);

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
        public DateTime(int year, Seasons season, int day, int hour, int minute)
        {
            _totalMinutes = (year - 1) * SeasonMinutes * 4 // Годы в минутах
                            + ((int)season) * SeasonMinutes // Сезоны в минутах
                            + (day - 1) * DayMinutes // Дни в минутах
                            + hour * 60 // Часы в минутах
                            + minute; // Минуты
        }

        /// <summary> Создаёт объект времени, задав общее количество минут. </summary>
        /// <param name="totalMinutes"> Общее количество минут. </param>
        public DateTime(int totalMinutes)
        {
            _totalMinutes = totalMinutes;
        }

        /// <summary> Добавляет указанное количество минут к текущему времени. </summary>
        /// <param name="minutes"> Количество минут для добавления. </param>
        public void AddMinutes(int minutes)
        {
            _totalMinutes += minutes;
        }

        #region ToString

        /// <summary> Преобразует текущую дату и время в строку. </summary>
        /// <returns> Строковое представление текущей даты и времени. </returns>
        public override string ToString()
        {
            return $"{Year} year {Season} {DayInSeason} day {TimeToString()}";
        }

        /// <summary> Преобразует текущую дату в строку. </summary>
        /// <returns> Строковое представление текущей даты. </returns>
        public string DateToString()
        {
            return $"{DayOfWeek} {DayInSeason} {Year}";
        }

        /// <summary> Преобразует время в строку с учётом выбранного формата отображения. </summary>
        /// <param name="is24HourFormat"> 
        /// Если true, время отображается в 24-часовом формате (например, "14:05"). 
        /// Если false, время отображается в 12-часовом формате с AM/PM (например, "02:05 PM"). 
        /// </param>
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
                string period = Hour >= 12 ? "PM" : "AM";
                int hour12 = Hour % 12;
                if (hour12 == 0) hour12 = 12; // Если час = 0 или 12, то в 12-часовом формате это будет 12
                var hourString = hour12.ToString("D2");
                var minuteString = Minute.ToString("D2");
                return $"{hourString}:{minuteString} {period}";
            }
        }

        #endregion
    }
}