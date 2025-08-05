using FlavorfulStory.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> UI-элемент уведомления о подборе предмета. </summary>
    public class ItemPickupNotificationView : BaseNotificationView
    {
        /// <summary> Иконка предмета. </summary>
        [SerializeField] private Image _icon;

        /// <summary> Инициализирует уведомление на основе данных о предмете. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not ItemCollectedSignal notificationData) return;

            var itemStack = notificationData.ItemStack;
            _label.text = $"x{itemStack.Number} {itemStack.Item.ItemName}";
            _icon.sprite = itemStack.Item.Icon;
        }
    }
}