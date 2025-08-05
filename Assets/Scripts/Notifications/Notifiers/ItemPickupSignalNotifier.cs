using FlavorfulStory.InventorySystem;
using Zenject;

namespace FlavorfulStory.Notifications.Notifiers
{
    /// <summary> Обрабатывает событие подбора предмета и отправляет уведомление. </summary>
    public class ItemPickupSignalNotifier : BaseSignalNotifier<ItemCollectedSignal>
    {
        /// <summary> Конструктор, передаёт зависимости базовому обработчику сигнала. </summary>
        /// <param name="notificationService"> Сервис, отвечающий за показ уведомлений. </param>
        /// <param name="signalBus"> Сигнальная шина Zenject для получения событий. </param>
        public ItemPickupSignalNotifier(INotificationService notificationService, SignalBus signalBus)
            : base(notificationService, signalBus)
        {
        }
    }
}