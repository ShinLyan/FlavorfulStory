
using System;

namespace FlavorfulStory.TimeManagement
{
    [Serializable]
    public class DateTime
    {
        private int _totalMinutes;
        private int _dayMinutes;
        private int _seasonMinutes;

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

        public void AddMinutes(int minutes)
        {
            _totalMinutes += minutes;
        }
        
        public int GetYear()
        {
            return _totalMinutes / (_seasonMinutes * 4) + 1;
        }

        public Seasons GetSeason()
        {
            int seasonIndex = (_totalMinutes % (_seasonMinutes * 4)) / _seasonMinutes;
            return (Seasons)seasonIndex;
        }

        public int GetDayInSeason()
        {
            return (_totalMinutes % _seasonMinutes) / _dayMinutes + 1;
        }

        public Days GetDayOfWeek()
        {
            var dayIndex = (_totalMinutes / _dayMinutes) % 7;
            return (Days)dayIndex;
        }

        public int GetHour()
        {
            return (_totalMinutes % _dayMinutes) / 60;
        }

        public int GetMinute()
        {
            return _totalMinutes % 60;
        }

        public int GetTotalWeeks()
        {
            return _totalMinutes / (_dayMinutes * 7);
        }

        public int GetTotalDays()
        {
            return _totalMinutes / _dayMinutes;
        }

        public bool IsDay()
        {
            int currentHour = GetHour();
            if (currentHour > 6 && currentHour < 22)
                return true;
            return false;
        }
        
        #endregion
        
        #region Overrides
        
        public static bool operator ==(DateTime dt1, DateTime dt2)
        {
            if (dt1 is null || dt2 is null)
                return false;
            return dt1._totalMinutes == dt2._totalMinutes;
        }

        public static bool operator !=(DateTime dt1, DateTime dt2)
        {
            return !(dt1 == dt2);
        }

        public static bool operator >(DateTime dt1, DateTime dt2)
        {
            return dt1._totalMinutes > dt2._totalMinutes;
        }

        public static bool operator <(DateTime dt1, DateTime dt2)
        {
            return dt1._totalMinutes < dt2._totalMinutes;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is DateTime other)
                return _totalMinutes == other._totalMinutes;
            return false;
        }

        // Переопределение метода GetHashCode
        public override int GetHashCode()
        {
            return _totalMinutes.GetHashCode();
        }
        
        #endregion
        
        #region ToString

        public override string ToString()
        {
            return $"{GetDayInSeason()}.{(int)GetSeason() + 1}.{GetYear()} {GetHour()}:{GetMinute()}";
        }

        public string DateToString()
        {
            return $"{GetDayOfWeek()} {GetDayInSeason()} {GetYear()}";
        }

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
