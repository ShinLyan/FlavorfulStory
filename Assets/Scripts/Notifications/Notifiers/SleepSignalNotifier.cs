using FlavorfulStory.Notifications.Data;
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

        /// <summary> Создаёт уведомление о наступлении ночи. </summary>
        /// <param name="signal"> Сигнал, содержащий время начала ночи. </param>
        /// <returns> Данные уведомления с указанием часа наступления ночи. </returns>
        protected override INotificationData CreateNotification(NightStartedSignal signal) =>
            new SleepNotificationData(signal.Time.Hour);
    }
}