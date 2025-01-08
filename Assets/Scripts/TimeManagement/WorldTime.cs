using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlavorfulStory.TimeManagement
{
    public class WorldTime : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField, Range(1, 28)] private int _date;
        [SerializeField] private Seasons _season;
        [SerializeField, Range(1, 99)] private int _year;
        [SerializeField, Range(0, 24)] private int _hour;
        [SerializeField, Range(0, 60)] private int _minutes;
        
        [Header("Tick settings")] 
        [SerializeField] private int _tickMinutesIncrease = 10;
        [SerializeField] private int _timeBetweenTicks = 1;
        
        private float _currentTimeBetweenTicks;
        private DateTime _dateTime;
        
        public static Action<DateTime> OnDateTimeChanged;
        public static Action OnGlobalDayReset;
        
        private void Awake()
        {
            _dateTime = new DateTime(_year, (int)_season, _date, _hour, _minutes);
            OnDateTimeChanged?.Invoke(_dateTime);
        }

        private void Update()
        {
            _currentTimeBetweenTicks += Time.deltaTime;

            if (_currentTimeBetweenTicks >= _timeBetweenTicks)
            {
                _currentTimeBetweenTicks = 0;
                AdvanceTime();
            }
        }

        private void AdvanceTime()
        {
            _dateTime.AddMinutes(_tickMinutesIncrease);
            if (_dateTime.GetHour() == 2)
            {
                OnGlobalDayReset?.Invoke();
                _dateTime.AddMinutes(60 * 4 - _dateTime.GetMinute());
            }
            OnDateTimeChanged?.Invoke(_dateTime);
        }
    }
}