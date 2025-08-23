using FlavorfulStory.Saving;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> Представление уведомления о завершении сохранения. </summary>
    public class SaveCompletedNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле с сообщением. </summary>
        [SerializeField] private TMP_Text _messageText;

        /// <summary> Инициализирует уведомление данными. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not SaveCompletedSignal) return;

            _messageText.text = "Your progress has been saved."; //TODO: localize
        }
    }
}