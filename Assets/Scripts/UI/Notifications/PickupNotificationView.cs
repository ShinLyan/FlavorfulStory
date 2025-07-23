using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlavorfulStory.UI.Notifications
{
    /// <summary> UI-элемент уведомления о подборе предмета. </summary>
    public class PickupNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле для отображения количества и названия предмета. </summary>
        [SerializeField] private TMP_Text _label;
        /// <summary> Иконка предмета. </summary>
        [SerializeField] private Image _icon;

        /// <summary> Высота уведомления, рассчитываемая по RectTransform. </summary>
        public override float Height => RectTransform.rect.height;

        /// <summary> Инициализирует уведомление на основе данных о предмете. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not PickupNotificationData pickupData) return;
            _label.text = $"x{pickupData.Amount} {pickupData.ItemName}";
            _icon.sprite = pickupData.Icon;
        }
    }
}