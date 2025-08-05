using System;
using FlavorfulStory.Notifications.Data;
using Zenject;

namespace FlavorfulStory.Notifications.Notifiers
{
    /// <summary> Базовый обработчик сигнала, создающий уведомление. </summary>
    /// <typeparam name="TSignal"> Тип обрабатываемого сигнала. </typeparam>
    public abstract class BaseSignalNotifier<TSignal> : IInitializable, IDisposable
    {
        /// <summary> Сервис отображения уведомлений. </summary>
        private readonly INotificationService _notificationService;

        /// <summary> Сигнальная шина Zenject. </summary>
        private readonly SignalBus _signalBus;

        /// <summary> Конструктор, внедряющий зависимости. </summary>
        /// <param name="notificationService"> Сервис, отвечающий за показ уведомлений. </param>
        /// <param name="signalBus"> Шина сигналов для подписки на события. </param>
        protected BaseSignalNotifier(INotificationService notificationService, SignalBus signalBus)
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
        private void OnSignalReceived(TSignal signal)
        {
            var notification = CreateNotification(signal);
            if (notification != null) _notificationService.Show(notification);
        }

        /// <summary> Создаёт данные уведомления на основе полученного сигнала. </summary>
        /// <param name="signal"> Сигнал, из которого нужно сформировать уведомление. </param>
        /// <returns> Объект с данными для уведомления. </returns>
        protected abstract INotificationData CreateNotification(TSignal signal);
    }
}