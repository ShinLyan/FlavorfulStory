using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> UI-элемент уведомления о подборе предмета. </summary>
    public class PickupNotificationView : BaseNotificationView
    {
        /// <summary> Иконка предмета. </summary>
        [SerializeField] private Image _icon;

        /// <summary> Инициализирует уведомление на основе данных о предмете. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not PickupNotificationData notificationData) return;

            _label.text = $"x{notificationData.Amount} {notificationData.ItemName}";
            _icon.sprite = notificationData.Icon;
        }
    }
}