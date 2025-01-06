using System;
using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField, Range(1, 28)] private int _date;
        [SerializeField, Range(1, 4)] private int _season;
        [SerializeField, Range(1, 99)] private int _year;
        [SerializeField, Range(0, 24)] private int _hour;
        [SerializeField, Range(0, 60)] private int _minutes;

        private DateTime _dateTime;
        
        [Header("Tick settings")] 
        public int TickMinutesIncrease = 10;
        public int TimeBetweenTicks = 1;
        private float _currentTimeBetweenTicks = 0;
        
        public static Action<DateTime> OnDateTimeChanged;

        private void Awake()
        {
            _dateTime = new DateTime(_year, _season, _date, _hour, _minutes);
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
            }
        }

        private void AdvanceTime()
        {
            _dateTime.AddMinutes(TickMinutesIncrease);

            print(_dateTime.ToString());
            OnDateTimeChanged?.Invoke(_dateTime);
        }
    }
}