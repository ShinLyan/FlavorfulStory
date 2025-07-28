using FlavorfulStory.InventorySystem;
using Zenject;

namespace FlavorfulStory.Notifications
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

        /// <summary> Создаёт уведомление о подборе предмета на основе полученного сигнала. </summary>
        /// <param name="signal"> Сигнал, содержащий информацию о подобранном предмете. </param>
        /// <returns> Данные для отображения уведомления. </returns>
        protected override INotificationData CreateNotification(ItemCollectedSignal signal)
        {
            var itemStack = signal.ItemStack;
            return new PickupNotificationData(itemStack.Number, itemStack.Item.Icon, itemStack.Item.ItemName);
        }
    }
}