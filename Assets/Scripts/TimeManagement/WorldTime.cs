using System;
using FlavorfulStory.EditorTools.Attributes;
using FlavorfulStory.Saving;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет глобальным игровым временем, изменяя его по тикам и вызывая события. </summary>
    public class WorldTime : MonoBehaviour, ISaveable
    {
        #region Fields

        /// <summary> Сколько игровых минут проходит за реальную секунду. </summary>
        [Tooltip("Сколько игровых минут проходит за реальную секунду."), SerializeField, SteppedRange(-50f, 100f, 5f)]
        private float _timeScale = 5f;

        /// <summary> Шаг времени. </summary>
        public static float TimeScale { get; private set; }

        /// <summary> Множитель скорости. </summary>
        public static float MovementSpeedMultiplier { get; private set; } = 1f;

        /// <summary> Сигнальная шина Zenject для отправки и получения событий. </summary>
        private SignalBus _signalBus;

        /// <summary> Текущее игровое время. </summary>
        public static DateTime CurrentGameTime { get; private set; }

        /// <summary> Предыдущее время. </summary>
        private DateTime _previousTime;

        /// <summary> Игра на паузе? </summary>
        public static bool IsPaused { get; private set; }

        //TODO: вот эти поля вынести в конфиг(static_data) и прокидывать через ConfigInstaller. *По аналогии с уведомлениями*
        /// <summary> Раз в сколько игровых минут происходит тик времени. </summary>
        private const float TimeBetweenTicks = 5f;

        /// <summary> Час начала нового дня. </summary>
        private const int DefaultDayStartHour = 6;

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

        public static WorldTime Instance { get; private set; } // TODO: ВРЕМЕННЫЙ КОСТЫЛЬ

        #endregion

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="signalBus"> Сигнальная шина Zenject для отправки и получения событий. </param>
        [Inject]
        private void Construct(SignalBus signalBus) => _signalBus = signalBus;

        /// <summary> Инициализировать начальное игровое время и подписаться на события. </summary>
        private void Awake()
        {
            CurrentGameTime = new DateTime(1, Season.Spring, 1, DefaultDayStartHour, 0);
            Instance = this;
        }

        /// <summary> Вызвать начальное обновление интерфейса. </summary>
        private void Start()
        {
            TimeScale = _timeScale;
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

            TimeScale = _timeScale;
            MovementSpeedMultiplier = CalculateMovementSpeedMultiplier();

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
            int previousHour = (int)_previousTime.Hour;
            int currentHour = (int)time.Hour;

            HandleHourTransition(previousHour, currentHour);
        }

        /// <summary> Обрабатывает переход между игровыми часами. </summary>
        /// <param name="previousHour"> Предыдущий час. </param>
        /// <param name="currentHour"> Текущий час. </param>
        private void HandleHourTransition(int previousHour, int currentHour)
        {
            const int DayEndHour = 2;
            const int MidnightHour = 0;
            const int ShopOpenHour = 8;
            const int ShopCloseHour = 20;

            if (previousHour == currentHour) return;

            switch (currentHour)
            {
                case DayEndHour:
                    BeginNewDay(10);
                    break;

                case MidnightHour:
                    _signalBus.Fire(new MidnightStartedSignal());
                    break;

                case ShopOpenHour:
                    _signalBus.Fire(new ShopStateChangedSignal(true, false));
                    break;

                case ShopCloseHour:
                    _signalBus.Fire(new ShopStateChangedSignal(false, false));
                    break;
            }
        }

        /// <summary> Расчёт множителя скорости. </summary>
        /// <returns> Вычисленное значение множителя скорости. </returns>
        private float CalculateMovementSpeedMultiplier()
        {
            const float BaseScale = 5f;
            const float MaxScale = 1000f;
            const float MaxMultiplier = 10f;

            float t = Mathf.InverseLerp(BaseScale, MaxScale, Mathf.Max(BaseScale, _timeScale));
            return Mathf.Lerp(1f, MaxMultiplier, Mathf.Pow(t, 0.01f));
        }

        /// <summary> Обновить игровое время до начала следующего дня. </summary>
        public void BeginNewDay(int dayStartHour = DefaultDayStartHour)
        {
            bool isSameDay = CurrentGameTime.Hour is >= 0f and < DefaultDayStartHour;
            int dayAdjustment = isSameDay ? 0 : 1;
            CurrentGameTime = new DateTime(CurrentGameTime.Year, CurrentGameTime.Season,
                CurrentGameTime.SeasonDay + dayAdjustment, dayStartHour, 0);

            OnDayEnded?.Invoke(CurrentGameTime);
            SavingWrapper.Save();
            _signalBus.Fire(new SaveCompletedSignal());
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

        #region ISaveable

        /// <summary> Сохраняет текущее игровое время. </summary>
        public object CaptureState() => CurrentGameTime;

        /// <summary> Восстанавливает игровое время из сохранённого состояния. </summary>
        /// <param name="state"> Сохранённое значение игрового времени. </param>
        public void RestoreState(object state) => CurrentGameTime = (DateTime)state;

        #endregion
    }
}