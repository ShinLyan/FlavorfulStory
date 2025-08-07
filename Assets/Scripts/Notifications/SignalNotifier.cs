using System;
using Zenject;

namespace FlavorfulStory.Notifications
{
    /// <summary> Обработчик сигнала, напрямую передающий сигнал как уведомление. </summary>
    /// <typeparam name="TSignal"> Тип сигнала, реализующий INotificationData. </typeparam>
    public class SignalNotifier<TSignal> : IInitializable, IDisposable
        where TSignal : INotificationData
    {
        /// <summary> Сервис отображения уведомлений. </summary>
        private readonly INotificationService _notificationService;

        /// <summary> Сигнальная шина Zenject. </summary>
        private readonly SignalBus _signalBus;

        /// <summary> Конструктор, внедряющий зависимости. </summary>
        /// <param name="notificationService"> Сервис, отвечающий за показ уведомлений. </param>
        /// <param name="signalBus"> Шина сигналов для подписки на события. </param>
        protected SignalNotifier(INotificationService notificationService, SignalBus signalBus)
        {
            _notificationService = notificationService;
            _signalBus = signalBus;
        }

        /// <summary> Подписка на сигнал при инициализации. </summary>
        public void Initialize() => _signalBus.Subscribe<TSignal>(OnSignalReceived);

        /// <summary> Отписка от сигнала при уничтожении. </summary>
        public void Dispose() => _signalBus.Unsubscribe<TSignal>(OnSignalReceived);

        /// <summary> Обработка полученного сигнала и отображение уведомления. </summary>
        /// <param name="signal"> Полученный сигнал. </param>
        private void OnSignalReceived(TSignal signal) => _notificationService.Show(signal);
    }
}