using FlavorfulStory.TimeManagement;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> Представление уведомления о сне от истощения. </summary>
    public class ExhaustedSleepNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле с сообщением. </summary>
        [SerializeField] private TMP_Text _messageText;

        /// <summary> Инициализирует уведомление данными. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not ExhaustedSleepSignal) return;

            _messageText.text = "You fell asleep from tiredness…"; //TODO: localize
        }
    }
}