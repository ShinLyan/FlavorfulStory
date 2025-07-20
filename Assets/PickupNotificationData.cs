using UnityEngine;

namespace FlavorfulStory
{
    public class PickupNotificationData : INotificationData
    {
        public NotificationType Type => NotificationType.ItemPickup;
        public string ItemID;
        public string ItemName;
        public int Amount;
        public Sprite Icon;
    }
}