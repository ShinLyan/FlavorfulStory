using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    public class TimeService
    {
        private DateTime _currentTime;
        private readonly int _sunriseHour;
        private readonly int _sunsetHour;

        public TimeService(TimeSettings timeSettings)
        {
            _sunriseHour = timeSettings.SunriseHour;
            _sunsetHour = timeSettings.SunsetHour;
        }

        public float CalculateSunAngle()
        {
            bool isDay = IsDayTime();
            float startDegree = isDay ? 0 : 180;
            int startHour = isDay ? _sunriseHour : _sunsetHour;
            int endHour = isDay ? _sunsetHour : _sunriseHour;
            int totalTimeHour = CalculateDifference(startHour, endHour);
            int elapsedTimeHour = CalculateDifference(startHour, WorldTime.GetCurrentGameTime().Hour);
            float currentMinutes = WorldTime.GetCurrentGameTime().Minute;

            float percentage = (elapsedTimeHour * 60 + currentMinutes) / (totalTimeHour * 60);
            return Mathf.Lerp(startDegree, startDegree + 180, percentage);
        }

        private bool IsDayTime() => WorldTime.GetCurrentGameTime().Hour >= _sunriseHour &&
                                    WorldTime.GetCurrentGameTime().Hour <= _sunsetHour;


        private int CalculateDifference(int from, int to)
        {
            int difference = to - from;
            return difference < 0 ? difference + 24 : difference;
        }
    }
}