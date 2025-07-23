using System;
using Zenject;
using FlavorfulStory.TimeManagement;

namespace FlavorfulStory.UI.Notifications
{
    /// <summary> Отправляет уведомление при наступлении ночи. </summary>
    public class SleepNotifier : IInitializable, IDisposable
    {
        /// <summary> Сервис отображения уведомлений. </summary>
        private readonly INotificationService _notificationService;
        
        private readonly SignalBus _signalBus;
        
        /// <summary> Инжект зависимости сервиса уведомлений. </summary>
        /// <param name="notificationService"> Сервис уведомлений. </param>
        protected SleepNotifier(INotificationService notificationService, SignalBus signalBus)
        {
            _notificationService = notificationService;
            _signalBus = signalBus;
        }

        /// <summary> Подписка на событие начала ночи. </summary>
        public void Initialize() => _signalBus.Subscribe<NightStartedSignal>(OnNightStarted);

        /// <summary> Отписка от события начала ночи. </summary>
        public void Dispose() => _signalBus.Unsubscribe<NightStartedSignal>(OnNightStarted);

        /// <summary> Обрабатывает сигнал наступления ночи и запускает показ уведомления. </summary>
        /// <param name="signal"> Сигнал, содержащий информацию о времени начала ночи. </param>
        private void OnNightStarted(NightStartedSignal signal)
        {
            _notificationService.Show(new SleepNotificationData()
            {
                Hour = (int) signal.Time.Hour
            });
        }
    }
}