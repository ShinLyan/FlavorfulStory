using FlavorfulStory.PlacementSystem;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> Отображение уведомления о невозможности демонтажа объекта. </summary>
    public class DismantleDeniedNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле с сообщением. </summary>
        [SerializeField] private TMP_Text _messageText;

        /// <summary> Инициализирует уведомление данными о невозможности демонтажа объекта. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not DismantleDeniedSignal notificationData) return;

            _messageText.text = notificationData.Message;
        }
    }
}