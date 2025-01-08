
using System;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Класс для взаимодействия с игровым временем.</summary>
    [Serializable]
    public class DateTime
    {
        /// <summary> Общее кол-во минут.</summary>
        private int _totalMinutes;
        /// <summary> Кол-во минут в одном дне.</summary>
        private int _dayMinutes;
        /// <summary> Кол-во минут в одном сезоне.</summary>
        private int _seasonMinutes;

        /// <summary> Конструктор.</summary>
        /// <param name="year">Год.</param>
        /// <param name="season">Сезон.</param>
        /// <param name="day">День в сезоне.</param>
        /// <param name="hour">Часы.</param>
        /// <param name="minute">Минуты.</param>
        public DateTime(int year, int season, int day, int hour, int minute)
        {
            _dayMinutes = 1440;
            _seasonMinutes = _dayMinutes * 28;
            
            _totalMinutes = ((year - 1) * _seasonMinutes * 4) // Годы в минутах
                           + ((season - 1) * _seasonMinutes)     // Сезоны в минутах
                           + ((day - 1) * _dayMinutes)   // Дни в минутах
                           + (hour * 60)          // Часы в минутах
                           + minute;
        }

        #region Methods

        /// <summary> Добавление минут. </summary>
        /// <param name="minutes"> Добавляемые минуты.</param>
        public void AddMinutes(int minutes)
        {
            _totalMinutes += minutes;
        }
        
        /// <summary> Получить год.</summary>
        /// <returns> Номер года.</returns>
        public int GetYear()
        {
            return _totalMinutes / (_seasonMinutes * 4) + 1;
        }

        /// <summary> Получить текущий сезон.</summary>
        /// <returns> Текущий сезон.</returns>
        public Seasons GetSeason()
        {
            int seasonIndex = (_totalMinutes % (_seasonMinutes * 4)) / _seasonMinutes;
            return (Seasons)seasonIndex;
        }

        /// <summary> Получить день в сезоне.</summary>
        /// <returns> Номер дня в сезоне.</returns>
        public int GetDayInSeason()
        {
            return (_totalMinutes % _seasonMinutes) / _dayMinutes + 1;
        }

        /// <summary> Получить день недели.</summary>
        /// <returns> Номер текущего года.</returns>
        public WeekDays GetDayOfWeek()
        {
            var dayIndex = (_totalMinutes / _dayMinutes) % 7;
            return (WeekDays)dayIndex;
        }

        /// <summary> Получить часы.</summary>
        /// <returns> Кол-во часов.</returns>
        public int GetHour()
        {
            return (_totalMinutes % _dayMinutes) / 60;
        }

        /// <summary> Получить минуты.</summary>
        /// <returns> Кол-во минут.</returns>
        public int GetMinute()
        {
            return _totalMinutes % 60;
        }

        /// <summary> Получить число прошедших недель.</summary>
        /// <returns> Кол-во прошедших недель.</returns>
        public int GetTotalWeeks()
        {
            return _totalMinutes / (_dayMinutes * 7);
        }

        /// <summary> Получить число прошедших дней.</summary>
        /// <returns> Кол-во прошедших дней.</returns>
        public int GetTotalDays()
        {
            return _totalMinutes / _dayMinutes;
        }
        
        #endregion
        
        #region Overrides
        
        /// <summary> Оператор равенства.</summary>
        /// <param name="dt1"> Первое время.</param>
        /// <param name="dt2"> Второе время.</param>
        /// <returns> Булево равенство времен.</returns>
        public static bool operator ==(DateTime dt1, DateTime dt2)
        {
            if (dt1 is null || dt2 is null)
                return false;
            return dt1._totalMinutes == dt2._totalMinutes;
        }
        
        /// <summary> Оператор неравенства.</summary>
        /// <param name="dt1"> Первое время.</param>
        /// <param name="dt2"> Второе время.</param>
        /// <returns> Булево неравенство времен.</returns>
        public static bool operator !=(DateTime dt1, DateTime dt2)
        {
            return !(dt1 == dt2);
        }

        /// <summary> Оператор больше.</summary>
        /// <param name="dt1"> Первое время.</param>
        /// <param name="dt2"> Второе время.</param>
        /// <returns> Булево сравнение времен.</returns>
        public static bool operator >(DateTime dt1, DateTime dt2)
        {
            return dt1._totalMinutes > dt2._totalMinutes;
        }

        /// <summary> Оператор меньше.</summary>
        /// <param name="dt1"> Первое время.</param>
        /// <param name="dt2"> Второе время.</param>
        /// <returns> Булево сравнение времен.</returns>
        public static bool operator <(DateTime dt1, DateTime dt2)
        {
            return dt1._totalMinutes < dt2._totalMinutes;
        }
        
        /// <summary> Равенство.</summary>
        /// <param name="obj"> Объект.</param>
        /// <returns> Булево равенство.</returns>
        public override bool Equals(object obj)
        {
            if (obj is DateTime other)
                return _totalMinutes == other._totalMinutes;
            return false;
        }

        /// <summary> Получение хэш-кода.</summary>
        /// <returns> Хэш-код.</returns>
        public override int GetHashCode()
        {
            return _totalMinutes.GetHashCode();
        }
        
        #endregion
        
        #region ToString

        /// <summary> Переопределение ToString().</summary>
        /// <returns> Строка с текущей датой и временем.</returns>
        public override string ToString()
        {
            return $"{GetDayInSeason()}.{(int)GetSeason() + 1}.{GetYear()} {GetHour()}:{GetMinute()}";
        }

        /// <summary> Перевод даты в строковый тип.</summary>
        /// <returns> Строка с датой.</returns>
        public string DateToString()
        {
            return $"{GetDayOfWeek()} {GetDayInSeason()} {GetYear()}";
        }

        /// <summary> Перевод временем в строковый тип.</summary>
        /// <returns> Строка с временем.</returns>
        public string TimeToString()
        {
            var currentHour = GetHour();
            var currentMinute = GetMinute();
            var hourString = currentHour.ToString();
            var minuteString = currentMinute.ToString();
            
            if (currentHour < 10)
                hourString = "0" + currentHour;
            if (currentMinute < 10)
                minuteString = "0" + currentMinute;
            
            return $"{hourString}:{minuteString}";
        }

        #endregion
    }
}
