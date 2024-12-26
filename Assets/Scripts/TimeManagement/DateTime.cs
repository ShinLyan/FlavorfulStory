using System;

namespace FlavorfulStory.TimeManagement
{
    [Serializable]
    public struct DateTime
    {
        #region Properties
        public Days Day { get; private set; }

        public int Date { get; private set;}

        public int Year { get; private set; }

        public int Hour { get; private set; }

        public int Minutes { get; private set; }

        public Seasons Season { get; private set; }

        public int TotalNumDays { get; private set; }

        public int TotalNumWeeks { get; private set; }

        public int CurrentWeek => TotalNumWeeks % 16 == 0 ? 16 : TotalNumWeeks % 16;
        #endregion
        
        #region Constructors

        public DateTime(int date, int season, int year, int hour, int minuteses)
        {
            Day = (Days)(date % 7);
            if (Day == 0) Day = (Days)7;

            Date = date;
            Season = (Seasons)season;
            Year = year;
            Hour = hour;
            Minutes = minuteses;

            TotalNumDays = date + (28 * ((int)Season - 1)) + (112 * (Year - 1));
            
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

            if (Date % 29 == 0)
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
    
    [Serializable]
    public enum Days
    {
        Null = 0,
        Mon = 1,
        Tue = 2,
        Wed = 3,
        Thu = 4, 
        Fri = 5, 
        Sat = 6,
        Sun = 7
    }

    [Serializable]
    public enum Seasons
    {
        Winter = 0,
        Spring = 1,
        Summer = 2,
        Autumn = 3
    }
}