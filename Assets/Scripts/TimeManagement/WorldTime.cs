using System;
using FlavorfulStory.EditorTools.Attributes;
using FlavorfulStory.Saving;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using Zenject;

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

        /// <summary> Предыдущее время. </summary>
        private DateTime _previousTime;

        /// <summary> Сигнальная шина Zenject для отправки и получения событий. </summary>
        private SignalBus _signalBus;

        /// <summary> Обертка системы сохранений. </summary>
        private static SavingWrapper _savingWrapper;

        /// <summary> Игра на паузе? </summary>
        public static bool IsPaused { get; private set; }

        /// <summary> Раз в сколько игровых минут происходит тик времени. </summary>
        private const float TimeBetweenTicks = 5f;

        /// <summary> Час начала нового дня. </summary>
        private const int DayStartHour = 6;

        /// <summary> Вызывается при изменении игрового времени. </summary>
        public static Action<DateTime> OnTimeUpdated;

        /// <summary> Вызывается при завершении игрового дня. </summary>
        public static Action<DateTime> OnDayEnded;

        /// <summary> Вызывается при заданном тике времени. </summary>
        public static Action<DateTime> OnTimeTick;

        /// <summary> Вызывается при паузе времени. </summary>
        public static Action OnTimePaused;

        /// <summary> Вызывается при снятии паузы времени. </summary>
        public static Action OnTimeUnpaused;

        #endregion

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="signalBus"> Сигнальная шина Zenject для отправки и получения событий. </param>
        /// <param name="savingWrapper"> Сигнальная шина. </param>
        [Inject]
        private void Construct(SignalBus signalBus, SavingWrapper savingWrapper)
        {
            _signalBus = signalBus;
            _savingWrapper = savingWrapper;
        }

        /// <summary> Инициализировать начальное игровое время и подписаться на события. </summary>
        private void Awake() => CurrentGameTime = new DateTime(1, Season.Spring, 1, DayStartHour, 0);

        /// <summary> Вызвать начальное обновление интерфейса. </summary>
        private void Start()
        {
            OnTimeUpdated?.Invoke(CurrentGameTime);
            OnTimeTick?.Invoke(CurrentGameTime);

            OnTimeTick += HandleTimeTick;
        }

        /// <summary> Очистить состояние и события при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            CurrentGameTime = default;
            IsPaused = false;

            OnTimeUpdated = null;
            OnDayEnded = null;
            OnTimeTick = null;
            OnTimePaused = null;
            OnTimeUnpaused = null;

            OnTimeTick -= HandleTimeTick;
        }

        /// <summary> Обновить игровое время при отсутствии паузы. </summary>
        private void Update()
        {
            if (IsPaused) return;

            _previousTime = CurrentGameTime;
            CurrentGameTime = CurrentGameTime.AddMinutes(Time.deltaTime * _timeScale);

            int previousTick = (int)(_previousTime.Minute / TimeBetweenTicks);
            int currentTick = (int)(CurrentGameTime.Minute / TimeBetweenTicks);

            if (currentTick != previousTick) OnTimeTick?.Invoke(CurrentGameTime);

            OnTimeUpdated?.Invoke(CurrentGameTime);
        }

        /// <summary> Обработчик, вызываемый на каждый тик времени. </summary>
        private void HandleTimeTick(DateTime time)
        {
            const int DayEndHour = 2;
            const int MidnightHour = 0;

            int previousHour = (int)_previousTime.Hour;
            int currentHour = (int)time.Hour;

            if (previousHour != DayEndHour && currentHour == DayEndHour) BeginNewDay();

            if (previousHour != MidnightHour && currentHour == MidnightHour)
                _signalBus.Fire(new MidnightStartedSignal());
        }

        /// <summary> Обновить игровое время до начала следующего дня. </summary>
        public static void BeginNewDay(int dayStartHour = 10)
        {
            bool isSameDay = CurrentGameTime.Hour is >= 0f and < DayStartHour;
            int dayAdjustment = isSameDay ? 0 : 1;
            CurrentGameTime = new DateTime(CurrentGameTime.Year, CurrentGameTime.Season,
                CurrentGameTime.SeasonDay + dayAdjustment, dayStartHour, 0);

            OnDayEnded?.Invoke(CurrentGameTime);
            _savingWrapper.Save();
        }

        /// <summary> Поставить игровое время на паузу. </summary>
        public static void Pause()
        {
            IsPaused = true;
            OnTimePaused?.Invoke();
        }

        /// <summary> Снять паузу с игрового времени. </summary>
        public static void Unpause()
        {
            IsPaused = false;
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