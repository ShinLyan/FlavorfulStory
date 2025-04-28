using System;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет глобальным игровым временем, изменяя его по тикам и вызывая события. </summary>
    public class WorldTime : MonoBehaviour, ISaveable
    {
        #region Fields

        [Header("Time Scale")]
        [Tooltip("Сколько игровых минут проходит за реальную секунду."), SerializeField, Range(-100f, 1000f)]
        private float _timeScale = 1f;

        [Tooltip("Раз в сколько игровых минут происходит тик времени."), SerializeField, Range(1, 10)]
        private float _timeBetweenTicks = 1f;

        /// <summary> Час начала нового дня. </summary>
        [Header("Day/night settings")] [Tooltip("Во сколько начинается новый день."), SerializeField, Range(0, 24)]
        private int _dayStartHour;

        /// <summary> Час окончания дня. </summary>
        [Tooltip("Во сколько заканчивается день."), SerializeField, Range(0, 24)]
        private int _dayEndHour;

        private const int NightStartHour = 18;

        /// <summary> Текущее игровое время. </summary>
        private static DateTime _currentGameTime;

        /// <summary> Игра на паузе? </summary>
        private static bool _isPaused;

        /// <summary> Вызывается при изменении игрового времени. </summary>
        public static Action<DateTime> OnTimeUpdated;

        /// <summary> Вызывается при завершении игрового дня. </summary>
        public static Action<DateTime> OnDayEnded;

        /// <summary> Вызывается при наступлении ночи. </summary>
        public static Action<DateTime> OnNightStarted;

        /// <summary> Вызывается при заданном тике времени. </summary>
        public static Action<DateTime> OnTimeTick;

        /// <summary> Вызывается при паузе времени. </summary>
        public static Action OnTimePaused;

        /// <summary> Вызывается при снятии паузы времени. </summary>
        public static Action OnTimeUnpaused;

        #endregion

        /// <summary> Инициализировать начальное игровое время. </summary>
        private void Awake() => _currentGameTime = new DateTime(1, Season.Spring, 1, _dayStartHour, 0);

        /// <summary> Вызвать обновление UI при старте. </summary>
        private void Start()
        {
            OnTimeUpdated?.Invoke(_currentGameTime);
            OnTimeTick?.Invoke(_currentGameTime);
        }

        /// <summary> Обновить игровое время, если не стоит пауза. </summary>
        private void Update()
        {
            if (_isPaused) return;

            var previousTime = _currentGameTime;
            float gameMinutesToAdd = Time.deltaTime * _timeScale;
            _currentGameTime = _currentGameTime.AddMinutes(gameMinutesToAdd);

            if (previousTime.Hour < NightStartHour && _currentGameTime.Hour >= NightStartHour)
                OnNightStarted?.Invoke(_currentGameTime);

            if (previousTime.Hour < _dayEndHour && _currentGameTime.Hour >= _dayEndHour)
            {
                BeginNewDay();
                OnDayEnded?.Invoke(_currentGameTime);
            }

            if ((int)_currentGameTime.Minute % _timeBetweenTicks == 0) OnTimeTick?.Invoke(_currentGameTime);


            OnTimeUpdated?.Invoke(_currentGameTime);
        }

        /// <summary> Обновляет время до начала нового дня в зависимости от текущего времени. </summary>
        private void BeginNewDay()
        {
            bool isSameDay = 0f <= _currentGameTime.Hour && _currentGameTime.Hour < _dayStartHour;
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
        public static void Pause()
        {
            _isPaused = true;
            OnTimePaused?.Invoke();
        }

        /// <summary> Снять паузу с игрового времени. </summary>
        public static void Unpause()
        {
            _isPaused = false;
            OnTimeUnpaused?.Invoke();
        }

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