using FlavorfulStory.TimeManagement;
using Zenject;

namespace FlavorfulStory.Notifications.Notifiers
{
    /// <summary> Отправляет уведомление при наступлении ночи. </summary>
    public class SleepSignalNotifier : BaseSignalNotifier<NightStartedSignal>
    {
        /// <summary> Конструктор, передаёт зависимости базовому классу. </summary>
        /// <param name="notificationService"> Сервис показа уведомлений. </param>
        /// <param name="signalBus"> Сигнальная шина для получения событий. </param>
        public SleepSignalNotifier(INotificationService notificationService, SignalBus signalBus)
            : base(notificationService, signalBus)
        {
        }
    }
}