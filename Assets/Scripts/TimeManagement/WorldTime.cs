using System;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет глобальным игровым временем, изменяя его по тикам и вызывая события. </summary>
    public class WorldTime : MonoBehaviour, ISaveable
    {
        /// <summary> Вызывается при изменении игрового времени. </summary>
        public static Action<DateTime> OnDateTimeChanged;

        /// <summary> Вызывается при завершении игрового дня. </summary>
        public static Action OnDayEnded;

        [Header("Tick settings")]
        /// <summary> Количество игровых минут, добавляемых за один тик. </summary>
        [Tooltip("Сколько минут проходит за один тик.")]
        [SerializeField]
        private int _tickMinutesIncrease = 10;

        /// <summary> Время в секундах между тиками. </summary>
        [Tooltip("Сколько реального времени длится один тик.")] [SerializeField]
        private float _timeBetweenTicks = 1;

        [Header("Day/night settings")]
        /// <summary> Час начала нового дня. </summary>
        [Tooltip("Во сколько начинается новый день.")]
        [SerializeField]
        private int _dayStartHour;

        /// <summary> Час окончания дня. </summary>
        [Tooltip("Во сколько заканчивается день.")] [SerializeField]
        private int _dayEndHour;

        /// <summary> Текущее время, прошедшее с момента последнего тика. </summary>
        private float _currentTimeBetweenTicks;

        /// <summary> Текущее игровое время. </summary>
        private DateTime _dateTime;

        /// <summary> Создаёт объект `DateTime` и инициализирует его значением начала первого дня. </summary>
        private void Awake()
        {
            // TODO: Если новая игра - инициализировать, иначе загрузить сохранённое время.
            _dateTime = new DateTime(1, Seasons.Spring, 1, _dayStartHour, 0);
        }

        /// <summary> Вызывает обновление UI при старте. </summary>
        private void Start()
        {
            OnDateTimeChanged?.Invoke(_dateTime);
        }

        /// <summary> Обновляет игровое время, проверяя, прошёл ли очередной тик. </summary>
        private void Update()
        {
            _currentTimeBetweenTicks += Time.deltaTime;

            if (_currentTimeBetweenTicks >= _timeBetweenTicks)
            {
                _currentTimeBetweenTicks = 0f;
                IncreaseTime();
            }
        }

        /// <summary> Увеличивает игровое время и проверяет завершение дня. </summary>
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
                0
            );

            OnDateTimeChanged?.Invoke(_dateTime);
        }

        #region Saving

        /// <summary> Сохраняет текущее игровое время. </summary>
        public object CaptureState()
        {
            return _dateTime;
        }

        /// <summary> Восстанавливает игровое время из сохранённого состояния. </summary>
        /// <param name="state"> Сохранённое значение игрового времени. </param>
        public void RestoreState(object state)
        {
            _dateTime = (DateTime)state;
        }

        #endregion
    }
}