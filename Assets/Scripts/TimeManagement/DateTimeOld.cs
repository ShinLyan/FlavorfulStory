using System;

namespace FlavorfulStory.TimeManagement
{
    [Serializable]
    public class DateTimeOld
    {
        #region Properties
        public Days Day { get;  set; }

        public int Date { get;  set;}

        public int Year { get;  set; }

        public int Hour { get;  set; }

        public int Minutes { get;  set; }

        public Seasons Season { get;  set; }

        public int TotalNumDays { get;  set; }

        public int TotalNumWeeks { get;  set; }

        public int CurrentWeek => TotalNumWeeks % 16 == 0 ? 16 : TotalNumWeeks % 16;

        private int _daysInSeason;
        
        #endregion
        
        #region Constructors

        public DateTimeOld(int date, int season, int year, int hour, int minutes)
        {
            Day = (Days)(date % 7);
            if (Day == 0) Day = (Days)7;

            Date = date;
            Season = (Seasons)season;
            Year = year;
            Hour = hour;
            Minutes = minutes;

            _daysInSeason = 28;

            TotalNumDays = date + (_daysInSeason * ((int)Season - 1)) + (_daysInSeason * 4 * (Year - 1));
            
            TotalNumWeeks = 1 + (TotalNumDays / 7);
        }
        
        #endregion
        
        #region Methods

        public void AdvanceMinutes(int minutesToAdvanceBy)
        {
            if (Minutes + minutesToAdvanceBy >= 60)
            {
                Minutes = (Minutes + minutesToAdvanceBy) % 60;
                AdvanceHour();
            }
            else Minutes += minutesToAdvanceBy;
        }

        public void AdvanceHour()
        {
            if (Hour + 1 == 24)
            {
                Hour = 0;
                AdvanceDay();
            }
            else Hour++;
        }

        public void AdvanceDay()
        {
            Day++;

            if (Day > (Days)7)
            {
                Day = (Days)1;
                TotalNumWeeks++;
            }

            Date++;

            if (Date % (_daysInSeason + 1) == 0)
            {
                AdvanceSeason();
                Date = 1;
            }

            TotalNumDays++;
        }

        public void AdvanceSeason()
        {
            if (Season == Seasons.Winter)
            {
                Season = Seasons.Spring;
                AdvanceYear();
            }
            else Season++;
        }

        public void AdvanceYear()
        {
            Date = 1;
            Year++;
        }
        
        #endregion

        #region Checks

        public bool IsNight() => Hour > 18 || Hour < 6;

        public bool IsWeekend() => Day > Days.Fri;
        
        public bool IsParticularDay(Days day) => day == Day;

        #endregion

        #region ToString

        public override string ToString()
        {
            return $"Date: {Date}, Season: {Season}, Year: {Year}, Hour: {Hour}, Minutes: {Minutes}, TotalNumDays: {TotalNumDays}, TotalNumWeeks: {TotalNumWeeks}";
        }

        public string DateToString()
        {
            return $"{Day} {Date} {Year.ToString("D2")}";
        }

        public string TimeToString()
        {
            return $"{Hour.ToString("D2")}:{Minutes.ToString("D2")}";
        }

        #endregion
    }
}