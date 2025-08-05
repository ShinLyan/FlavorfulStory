using UnityEngine;

namespace FlavorfulStory.Notifications.Data
{
    /// <summary> Данные для уведомления о подобранном предмете. </summary>
    public class ItemPickupNotificationData : INotificationData
    {
        /// <summary> Тип уведомления. </summary>
        public NotificationType Type => NotificationType.ItemPickup;

        /// <summary> Количество предметов. </summary>
        public int Amount { get; }

        /// <summary> Иконка предмета. </summary>
        public Sprite Icon { get; }

        /// <summary> Название предмета. </summary>
        public string ItemName { get; }

        /// <summary> Конструктор с параметрами. </summary>
        /// <param name="amount"> Количество предметов. </param>
        /// <param name="icon"> Иконка предмета. </param>
        /// <param name="itemName"> Название предмета. </param>
        public ItemPickupNotificationData(int amount, Sprite icon, string itemName)
        {
            Amount = amount;
            Icon = icon;
            ItemName = itemName;
        }
    }
}