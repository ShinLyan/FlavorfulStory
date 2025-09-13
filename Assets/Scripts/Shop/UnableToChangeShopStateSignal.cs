using FlavorfulStory.Notifications;
using FlavorfulStory.TimeManagement;

namespace FlavorfulStory.Shop
{
    /// <summary> Сигнал изменения состояния магазина. </summary>
    public class UnableToChangeShopStateSignal : INotificationData
    {
        public NotificationType Type => NotificationType.UnableToChangeShopState;

        /// <summary> Состояние магазина (true — открыт, false — закрыт). </summary>
        public bool IsOpen { get; }

        /// <summary> Время, когда можно будет изменить состояние. </summary>
        public DateTime EnableToChangeTime { get; }

        /// <summary> Создает новый сигнал изменения состояния магазина. </summary>
        /// <param name="isOpen"> Новое состояние магазина. </param>
        /// <param name="changeTime"> Время, когда можно будет изменить состояние. </param>
        public UnableToChangeShopStateSignal(bool isOpen, DateTime changeTime)
        {
            IsOpen = isOpen;
            EnableToChangeTime = changeTime;
        }
    }
}