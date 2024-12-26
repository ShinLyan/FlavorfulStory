using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlavorfulStory.TimeManagement
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Settings")] 
        [Range(1, 28)] public int dateInMonths;
        [Range(1, 4)] public int season;
        [Range(1, 99)] public int year;
        [Range(0, 24)] public int hour;
        [Range(0, 6)] public int minutes;

        private DateTime _dateTime;
        
        [Header("Tick settings")] 
        public int TickMinutesIncrease = 10;
        public int TimeBetweenTicks = 1;
        private float _currentTimeBetweenTicks = 0;
        
        public static Action<DateTime> OnDateTimeChanged;

        private void Awake()
        {
            _dateTime = new DateTime(dateInMonths, season, year, hour, minutes);
        }

        private void Start()
        {
            OnDateTimeChanged?.Invoke(_dateTime);
        }

        private void Update()
        {
            _currentTimeBetweenTicks += Time.deltaTime;

            if (_currentTimeBetweenTicks >= TimeBetweenTicks)
            {
                _currentTimeBetweenTicks = 0;
                AdvanceTime();
                print(_dateTime.ToString());
            }
        }

        private void AdvanceTime()
        {
            _dateTime.AdvanceMinutes(TickMinutesIncrease);
            
            OnDateTimeChanged?.Invoke(_dateTime);
        }
    }
}