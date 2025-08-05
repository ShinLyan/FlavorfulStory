using FlavorfulStory.TimeManagement;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> UI-элемент уведомления о наступлении ночи. </summary>
    public class NightStartedNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле с сообщением. </summary>
        [SerializeField] private TMP_Text _messageText;

        /// <summary> Инициализирует уведомление данными о ночи. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not NightStartedSignal notificationData) return;

            _messageText.text = "Night had fallen. It's already 18 o'clock";
        }
    }
}