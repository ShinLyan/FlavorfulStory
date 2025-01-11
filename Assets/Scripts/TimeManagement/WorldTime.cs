using System;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Глобальное игровое время. </summary>
    public class WorldTime : MonoBehaviour, ISaveable
    {
        /// <summary> Изменение игрового времени за один тик. </summary>
        [Header("Tick settings")]
        [Tooltip("Сколько минут проходит за один тик.")]
        [SerializeField] private int _tickMinutesIncrease = 10;

        /// <summary> Время между тиками. </summary>
        [Tooltip("Сколько реального времени длится один тик.")]
        [SerializeField] private float _timeBetweenTicks = 1;
        
        /// <summary> Во сколько начинается новый день. </summary>
        [Header("Day/night settings")]
        [Tooltip("Во сколько начинается новый день.")]
        [SerializeField] private int _dayStartHour;
        
        /// <summary> Во сколько заканчивается день. </summary>
        [Tooltip("Во сколько заканчивается день.")]
        [SerializeField] private int _dayEndHour;
        
        /// <summary> Время между тиками. </summary>
        private float _currentTimeBetweenTicks;
        
        /// <summary> Объект DateTime. </summary>
        private DateTime _dateTime;

        private ISaveable _saveableImplementation;

        /// <summary> Событие изменения времени. </summary>
        public static Action<DateTime> OnDateTimeChanged;
        
        /// <summary> Событие окончания дня. </summary>
        public static Action OnDayEnded;
        
        /// <summary> Создание объекта DateTime. </summary>
        private void Awake()
        {
            // TODO: Если новая игра - инициализировать, иначе - нет.
            _dateTime = new DateTime(1, Seasons.Spring, 1, _dayStartHour, 0);
            OnDateTimeChanged?.Invoke(_dateTime);
        }
        
        /// <summary> Вычисление игрового времени. </summary>
        private void Update()
        {
            _currentTimeBetweenTicks += Time.deltaTime;

            if (_currentTimeBetweenTicks >= _timeBetweenTicks)
            {
                _currentTimeBetweenTicks = 0f;
                IncreaseTime();
            }
        }

        /// <summary> Увеличить время. </summary>
        private void IncreaseTime()
        {
            _dateTime.AddMinutes(_tickMinutesIncrease);
            if (_dateTime.Hour == _dayEndHour)
            {
                OnDayEnded?.Invoke();
                StartNewDay();
            }

            OnDateTimeChanged?.Invoke(_dateTime);
        }
        
        /// <summary> Обновляет время до начала нового дня в зависимости от текущего времени. </summary>
        public void StartNewDay()
        {
            bool isSameDay = _dateTime.Hour < _dayStartHour;
            int dayAdjustment = isSameDay ? 0 : 1;

            _dateTime = new DateTime(
                _dateTime.Year,
                _dateTime.Season,
                _dateTime.DayInSeason + dayAdjustment,
                _dayStartHour,
                minute: 0
            );

            OnDateTimeChanged?.Invoke(_dateTime);
        }

        #region Saving
        public object CaptureState()
        {
            return _dateTime;
        }

        public void RestoreState(object state)
        {
            _dateTime = (DateTime) state;
        }
        #endregion
    }
    
}