using System;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет глобальным игровым временем, изменяя его по тикам и вызывая события. </summary>
    public class WorldTime : MonoBehaviour, ISaveable
    {
        #region Fields

        /// <summary> Количество игровых минут, добавляемых за один тик. </summary>
        [Header("Tick settings")] [Tooltip("Сколько минут проходит за один тик."), SerializeField, Range(0.1f, 20f)]
        private int _minutesPerTick = 10;

        /// <summary> Время в секундах между тиками. </summary>
        [Tooltip("Сколько реального времени длится один тик."), SerializeField, Range(0.1f, 20f)]
        private float _timeBetweenTicks = 1;

        /// <summary> Час начала нового дня. </summary>
        [Header("Day/night settings")]
        [Tooltip("Во сколько начинается новый день."), SerializeField, Range(0, 24)]
        private int _dayStartHour;

        /// <summary> Час окончания дня. </summary>
        [Tooltip("Во сколько заканчивается день."), SerializeField, Range(0, 24)]
        private int _dayEndHour;

        /// <summary> Текущее игровое время. </summary>
        private static DateTime _currentGameTime;

        /// <summary> Время, прошедшее с момента последнего тика. </summary>
        private float _elapsedTime;

        /// <summary> Игра на паузе? </summary>
        private static bool _isPaused;

        /// <summary> Вызывается при изменении игрового времени. </summary>
        public static Action<DateTime> OnTimeUpdated;

        /// <summary> Вызывается при завершении игрового дня. </summary>
        public static Action<DateTime> OnDayEnded;

        #endregion

        /// <summary> Инициализировать начальное игровое время. </summary>
        private void Awake() => _currentGameTime = new DateTime(1, Season.Spring, 1, _dayStartHour, 0);

        /// <summary> Вызвать обновление UI при старте. </summary>
        private void Start() => OnTimeUpdated?.Invoke(_currentGameTime);

        /// <summary> Обновить игровое время, если не стоит пауза. </summary>
        private void Update()
        {
            if (_isPaused) return;

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < _timeBetweenTicks) return;

            _elapsedTime = 0f;
            IncreaseTime();
        }

        /// <summary> Увеличить игровое время и проверить завершение дня. </summary>
        private void IncreaseTime()
        {
            print(_currentGameTime);

            _currentGameTime.AddMinutes(_minutesPerTick);

            if (_currentGameTime.Hour == _dayEndHour)
            {
                BeginNewDay();
                OnDayEnded?.Invoke(_currentGameTime);
            }

            OnTimeUpdated?.Invoke(_currentGameTime);
        }

        /// <summary> Обновляет время до начала нового дня в зависимости от текущего времени. </summary>
        private void BeginNewDay()
        {
            bool isSameDay = _currentGameTime.Hour < _dayStartHour;
            int dayAdjustment = isSameDay ? 0 : 1;

            _currentGameTime = new DateTime(
                _currentGameTime.Year,
                _currentGameTime.Season,
                _currentGameTime.SeasonDay + dayAdjustment,
                _dayStartHour,
                0
            );
        }

        /// <summary> Поставить игровое время на паузу. </summary>
        public static void Pause() => _isPaused = true;

        /// <summary> Снять паузу с игрового времени. </summary>
        public static void Unpause() => _isPaused = false;

        /// <summary> Получить текущее игровое время. </summary>
        /// <returns> Текущее игрвоое время. </returns>
        public static DateTime GetCurrentGameTime() => _currentGameTime;

        #region Saving

        /// <summary> Сохраняет текущее игровое время. </summary>
        public object CaptureState() => _currentGameTime;

        /// <summary> Восстанавливает игровое время из сохранённого состояния. </summary>
        /// <param name="state"> Сохранённое значение игрового времени. </param>
        public void RestoreState(object state) => _currentGameTime = (DateTime)state;

        #endregion
    }
}