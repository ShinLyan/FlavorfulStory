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
    }
}