using System;
using FlavorfulStory.EditorTools.Attributes;
using FlavorfulStory.Saving;
using UnityEngine;

// TODO: Актуализировать под Zenject
namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет глобальным игровым временем, изменяя его по тикам и вызывая события. </summary>
    public class WorldTime : MonoBehaviour, ISaveable
    {
        #region Fields

        /// <summary> Сколько игровых минут проходит за реальную секунду. </summary>
        [Header("Time Scale")]
        [Tooltip("Сколько игровых минут проходит за реальную секунду."), SerializeField, SteppedRange(-100f, 1000f, 5f)]
        private float _timeScale = 1f;

        /// <summary> Текущее игровое время. </summary>
        public static DateTime CurrentGameTime { get; private set; }

        /// <summary> Игра на паузе? </summary>
        private static bool _isPaused;

        /// <summary> Раз в сколько игровых минут происходит тик времени. </summary>
        private const float TimeBetweenTicks = 5f;

        /// <summary> Час начала нового дня. </summary>
        private const int DayStartHour = 6;

        /// <summary> Час окончания дня. </summary>
        private const int DayEndHour = 2;

        /// <summary> Час начала ночи. </summary>
        private const int NightStartHour = 18;

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

        /// <summary> Инициализировать начальное игровое время и подписаться на события. </summary>
        private void Awake() => CurrentGameTime = new DateTime(1, Season.Spring, 1, DayStartHour, 0);

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
            CurrentGameTime = CurrentGameTime.AddMinutes(Time.deltaTime * _timeScale);

            if (previousTime.Hour < NightStartHour && CurrentGameTime.Hour >= NightStartHour)
                OnNightStarted?.Invoke(CurrentGameTime);

            int previousTick = (int)(previousTime.Minute / TimeBetweenTicks);
            int currentTick = (int)(CurrentGameTime.Minute / TimeBetweenTicks);

            if (currentTick != previousTick) OnTimeTick?.Invoke(CurrentGameTime);

            OnTimeUpdated?.Invoke(CurrentGameTime);

            if (previousTime.Hour < DayEndHour && CurrentGameTime.Hour >= DayEndHour) BeginNewDay();
        }

        /// <summary> Обновить игровое время до начала следующего дня. </summary>
        public static void BeginNewDay(int dayStartHour = 10)
        {
            bool isSameDay = CurrentGameTime.Hour is >= 0f and < DayStartHour;
            int dayAdjustment = isSameDay ? 0 : 1;
            CurrentGameTime = new DateTime(CurrentGameTime.Year, CurrentGameTime.Season,
                CurrentGameTime.SeasonDay + dayAdjustment, dayStartHour, 0);

            OnDayEnded?.Invoke(CurrentGameTime);
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

        #region Saving

        /// <summary> Сохраняет текущее игровое время. </summary>
        public object CaptureState() => CurrentGameTime;

        /// <summary> Восстанавливает игровое время из сохранённого состояния. </summary>
        /// <param name="state"> Сохранённое значение игрового времени. </param>
        public void RestoreState(object state) => CurrentGameTime = (DateTime)state;

        #endregion
    }
}