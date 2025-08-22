using FlavorfulStory.Saving;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    public class SaveCompletedNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле с сообщением. </summary>
        [SerializeField] private TMP_Text _messageText;

        public override void Initialize(INotificationData data)
        {
            if (data is not SaveCompletedSignal) return;

            _messageText.text = "Your progress has been saved.";
        }
    }
}