using FlavorfulStory.Notifications.Data;
using FlavorfulStory.QuestSystem;
using Zenject;

namespace FlavorfulStory.Notifications.Notifiers
{
    /// <summary> Обработчик сигнала добавления квеста – создаёт уведомление. </summary>
    public class QuestAddedSignalNotifier : BaseSignalNotifier<QuestAddedSignal>
    {
        /// <summary> Конструктор, передающий зависимости базовому классу. </summary>
        /// <param name="notificationService"> Сервис для отображения уведомлений. </param>
        /// <param name="signalBus"> Шина сигналов Zenject. </param>
        public QuestAddedSignalNotifier(INotificationService notificationService, SignalBus signalBus)
            : base(notificationService, signalBus)
        {
        }

        /// <summary> Создаёт уведомление на основе полученного сигнала. </summary>
        /// <param name="signal"> Сигнал о добавлении квеста. </param>
        /// <returns> Данные уведомления с названием квеста. </returns>
        protected override INotificationData CreateNotification(QuestAddedSignal signal) =>
            new QuestAddedNotificationData(signal.QuestName);
    }
}