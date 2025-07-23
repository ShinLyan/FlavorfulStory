using UnityEngine;

namespace FlavorfulStory.UI.Notifications
{
    /// <summary> Данные для уведомления о подобранном предмете. </summary>
    public struct PickupNotificationData : INotificationData
    {
        /// <summary> Тип уведомления. </summary>
        public NotificationType Type => NotificationType.ItemPickup;

        /// <summary> Название предмета. </summary>
        public string ItemName;

        /// <summary> Количество предметов. </summary>
        public int Amount;

        /// <summary> Иконка предмета. </summary>
        public Sprite Icon;
    }
}