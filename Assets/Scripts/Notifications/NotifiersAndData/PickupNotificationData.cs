using UnityEngine;

namespace FlavorfulStory.Notifications
{
    /// <summary> Данные для уведомления о подобранном предмете. </summary>
    public struct PickupNotificationData : INotificationData
    {
        /// <summary> Тип уведомления. </summary>
        public NotificationType Type => NotificationType.ItemPickup;

        /// <summary> Название предмета. </summary>
        public string ItemName { get; }

        /// <summary> Количество предметов. </summary>
        public int Amount { get; }

        /// <summary> Иконка предмета. </summary>
        public Sprite Icon { get; }

        public PickupNotificationData(int amount, Sprite icon, string itemName)
        {
            Amount = amount;
            Icon = icon;
            ItemName = itemName;
        }
    }
}