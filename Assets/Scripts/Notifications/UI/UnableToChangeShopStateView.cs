using FlavorfulStory.Shop;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    public class UnableToChangeShopStateView : BaseNotificationView
    {
        /// <summary> Текстовое поле с сообщением. </summary>
        [SerializeField] private TMP_Text _messageText;

        public override void Initialize(INotificationData data)
        {
            if (data is not UnableToChangeShopStateSignal notificationData) return;

            _messageText.text = !notificationData.IsOpen
                ? $"You can open a store in {notificationData.EnableToChangeTime.TimeToString()}"
                : $"You can close a store in {notificationData.EnableToChangeTime.TimeToString()}";
        }
    }
}