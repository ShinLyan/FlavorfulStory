using System;
using FlavorfulStory.Saving;
using UnityEngine;

// TODO: Актуализировать под Zenject
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

        /// <summary> Час начала ночи. </summary>
        private const int NightStartHour = 18;

        /// <summary> Текущее игровое время. </summary>
        public static DateTime CurrentGameTime { get; private set; }

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

        /// <summary> Событие, вызываемое при принудительном завершении дня. </summary>
        private static Action OnForceEndDay;

        #endregion

        /// <summary> Инициализировать начальное игровое время и подписаться на события. </summary>
        private void Awake()
        {
            CurrentGameTime = new DateTime(1, Season.Spring, 1, _dayStartHour, 0);
            OnForceEndDay += BeginNewDay;
        }

        /// <summary> Вызвать начальное обновление интерфейса. </summary>
        private void Start()
        {
            OnTimeUpdated?.Invoke(CurrentGameTime);
            OnTimeTick?.Invoke(CurrentGameTime);
        }

        /// <summary> Очистить состояние и события при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            CurrentGameTime = default;
            _isPaused = false;

            OnTimeUpdated = null;
            OnDayEnded = null;
            OnNightStarted = null;
            OnTimeTick = null;
            OnTimePaused = null;
            OnTimeUnpaused = null;
        }

        /// <summary> Обновить игровое время при отсутствии паузы. </summary>
        private void Update()
        {
            if (_isPaused) return;

            var previousTime = CurrentGameTime;
            float gameMinutesToAdd = Time.deltaTime * _timeScale;
            CurrentGameTime = CurrentGameTime.AddMinutes(gameMinutesToAdd);

            if (previousTime.Hour < NightStartHour && CurrentGameTime.Hour >= NightStartHour)
                OnNightStarted?.Invoke(CurrentGameTime);

            if (previousTime.Hour < _dayEndHour && CurrentGameTime.Hour >= _dayEndHour) BeginNewDay();

            if ((int)CurrentGameTime.Minute % _timeBetweenTicks == 0) OnTimeTick?.Invoke(CurrentGameTime);

            OnTimeUpdated?.Invoke(CurrentGameTime);
        }

        /// <summary> Обновить игровое время до начала следующего дня. </summary>
        private void BeginNewDay()
        {
            bool isSameDay = 0f <= CurrentGameTime.Hour && CurrentGameTime.Hour < _dayStartHour;
            int dayAdjustment = isSameDay ? 0 : 1;
            CurrentGameTime = new DateTime(
                CurrentGameTime.Year,
                CurrentGameTime.Season,
                CurrentGameTime.SeasonDay + dayAdjustment,
                _dayStartHour,
                0
            );

            OnDayEnded?.Invoke(CurrentGameTime);
            // _dayEndManager.RequestEndDay(() => { });
        }

        /// <summary> Принудительно завершить текущий день. </summary>
        public static void ForceEndDay() => OnForceEndDay?.Invoke();

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

        #region Saving

        /// <summary> Сохраняет текущее игровое время. </summary>
        public object CaptureState() => CurrentGameTime;

        /// <summary> Восстанавливает игровое время из сохранённого состояния. </summary>
        /// <param name="state"> Сохранённое значение игрового времени. </param>
        public void RestoreState(object state) => CurrentGameTime = (DateTime)state;

        #endregion
    }
}