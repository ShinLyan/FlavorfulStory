using System;
using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Глобальное игровое время.</summary>
    public class WorldTime : MonoBehaviour
    {
        [Header("Settings")]
        /// <summary> Дата.</summary>
        [SerializeField, Range(1, 28)] private int _date;
        
        /// <summary> Сезон.</summary>
        [SerializeField] private Seasons _season;
        
        /// <summary> Год.</summary>
        [SerializeField, Range(1, 99)] private int _year;
        
        /// <summary> Час.</summary>
        [SerializeField, Range(0, 24)] private int _hour;

        /// <summary> Минуты.</summary>
        [SerializeField, Range(0, 60)] private int _minutes;

        /// <summary> Изменение игрового времени за один тик.</summary>
        [Header("Tick settings")]
        [Tooltip("Сколько минут проходит за один тик.")]
        [SerializeField] private int _tickMinutesIncrease = 10;

        /// <summary> Время между тиками.</summary>
        [Tooltip("Сколько реального времени длится один тик.")]
        [SerializeField] private int _timeBetweenTicks = 1;
        
        /// <summary> Время между тиками.</summary>
        private float _currentTimeBetweenTicks;
        
        /// <summary> Объект DateTime.</summary>
        private DateTime _dateTime;
        
        public static Action<DateTime> OnDateTimeChanged;
        public static Action OnGlobalDayReset;
        
        /// <summary> Создание объекта DateTime.</summary>
        private void Awake()
        {
            _dateTime = new DateTime(_year, (int)_season, _date, _hour, _minutes);
            OnDateTimeChanged?.Invoke(_dateTime);
        }

        /// <summary> Вычисление игрового времени.</summary>
        private void Update()
        {
            _currentTimeBetweenTicks += Time.deltaTime;

            if (_currentTimeBetweenTicks >= _timeBetweenTicks)
            {
                _currentTimeBetweenTicks = 0;
                IncreaseTime();
            }
        }

        /// <summary> Увеличить время.</summary>
        private void IncreaseTime()
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