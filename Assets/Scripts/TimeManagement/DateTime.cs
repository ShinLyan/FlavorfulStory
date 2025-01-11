
using System;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Класс для взаимодействия с игровым временем. </summary>
    [Serializable]
    public struct DateTime
    {
        /// <summary> Общее количество минут. </summary>
        private int _totalMinutes;
        
        /// <summary> Количество дней. </summary>
        private const int DaysCount = 28;
        
        /// <summary> Количество минут в одном дне. </summary>
        private const int DayMinutes = 60 * 24;
        
        /// <summary> Количество минут в одном сезоне. </summary>
        private const int SeasonMinutes = DayMinutes * DaysCount;

        /// <summary> Конструктор. </summary>
        /// <param name="year"> Год. </param>
        /// <param name="season"> Сезон. </param>
        /// <param name="day"> День в сезоне. </param>
        /// <param name="hour"> Часы. </param>
        /// <param name="minute"> Минуты. </param>
        public DateTime(int year, Seasons season, int day, int hour, int minute)
        {
            _totalMinutes = (year - 1) * SeasonMinutes * 4 // Годы в минутах
                           + ((int)season) * SeasonMinutes    // Сезоны в минутах
                           + (day - 1) * DayMinutes  // Дни в минутах
                           + hour * 60          // Часы в минутах
                           + minute;
        }

        public DateTime(int totalMinutes)
        {
            _totalMinutes = totalMinutes;
        }

        #region Properties
        
        /// <summary> Получить год. </summary>
        public int Year => _totalMinutes / (SeasonMinutes * 4) + 1;

        /// <summary> Получить текущий сезон. </summary>
        public Seasons Season => (Seasons)(_totalMinutes % (SeasonMinutes * 4) / SeasonMinutes);

        /// <summary> Получить день в сезоне. </summary>
        public int DayInSeason => _totalMinutes % SeasonMinutes / DayMinutes + 1;

        /// <summary> Получить день недели. </summary>
        public WeekDays DayOfWeek => (WeekDays)(_totalMinutes / DayMinutes % 7);

        /// <summary> Получить часы. </summary>
        public int Hour => _totalMinutes % DayMinutes / 60;

        /// <summary> Получить минуты. </summary>
        public int Minute => _totalMinutes % 60;

        /// <summary> Получить число прошедших недель. </summary>
        public int TotalWeeks => _totalMinutes / (DayMinutes * 7);

        /// <summary> Получить число прошедших дней. </summary>
        public int TotalDays => _totalMinutes / DayMinutes;

        #endregion
        
        #region Methods

        /// <summary> Добавление минут. </summary>
        /// <param name="minutes"> Добавляемые минуты. </param>
        public void AddMinutes(int minutes)
        {
            _totalMinutes += minutes;
        }
        
        #endregion
        
        #region Overrides
        //
        // /// <summary> Оператор равенства. </summary>
        // /// <param name="dt1"> Первое время. </param>
        // /// <param name="dt2"> Второе время. </param>
        // /// <returns> Булево равенство времен. </returns>
        // public static bool operator ==(DateTime dt1, DateTime dt2)
        // {
        //     if (dt1 is null || dt2 is null)
        //         return false;
        //     return dt1._totalMinutes == dt2._totalMinutes;
        // }
        //
        // /// <summary> Оператор неравенства. </summary>
        // /// <param name="dt1"> Первое время. </param>
        // /// <param name="dt2"> Второе время. </param>
        // /// <returns> Булево неравенство времен. </returns>
        // public static bool operator !=(DateTime dt1, DateTime dt2)
        // {
        //     return !(dt1 == dt2);
        // }
        //
        // /// <summary> Оператор больше. </summary>
        // /// <param name="dt1"> Первое время. </param>
        // /// <param name="dt2"> Второе время. </param>
        // /// <returns> Булево сравнение времен. </returns>
        // public static bool operator >(DateTime dt1, DateTime dt2)
        // {
        //     return dt1._totalMinutes > dt2._totalMinutes;
        // }
        //
        // /// <summary> Оператор меньше. </summary>
        // /// <param name="dt1"> Первое время. </param>
        // /// <param name="dt2"> Второе время. </param>
        // /// <returns> Булево сравнение времен. </returns>
        // public static bool operator <(DateTime dt1, DateTime dt2)
        // {
        //     return dt1._totalMinutes < dt2._totalMinutes;
        // }
        //
        // /// <summary> Равенство. </summary>
        // /// <param name="obj"> Объект. </param>
        // /// <returns> Булево равенство. </returns>
        // public override bool Equals(object obj)
        // {
        //     if (obj is DateTime other)
        //         return _totalMinutes == other._totalMinutes;
        //     return false;
        // }
        //
        // /// <summary> Получение хэш-кода. </summary>
        // /// <returns> Хэш-код. </returns>
        // public override int GetHashCode()
        // {
        //     return _totalMinutes.GetHashCode();
        // }
        
        #endregion
        
        #region ToString

        /// <summary> Переопределение ToString(). </summary>
        /// <returns> Строка с текущей датой и временем. </returns>
        public override string ToString()
        {
            return $"{Year} year {Season} {DayInSeason} day {TimeToString()}" ;
        }

        /// <summary> Перевод даты в строковый тип. </summary>
        /// <returns> Строка с датой. </returns>
        public string DateToString()
        {
            return $"{DayOfWeek} {DayInSeason} {Year}";
        }
        
        /// <summary> Преобразует время в строковый формат в зависимости от заданного формата отображения. </summary>
        /// <param name="is24HourFormat"> Если true, время будет возвращено в 24-часовом формате (например, "14:05").
        /// Если false, время будет возвращено в 12-часовом формате с указанием AM/PM (например, "02:05 PM"). </param>
        /// <returns> Строковое представление времени в указанном формате. </returns> 
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
                // AM/PM формат
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
