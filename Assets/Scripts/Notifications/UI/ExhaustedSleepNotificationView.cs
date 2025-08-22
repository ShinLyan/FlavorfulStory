using FlavorfulStory.TimeManagement;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    public class ExhaustedSleepNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле с сообщением. </summary>
        [SerializeField] private TMP_Text _messageText;

        public override void Initialize(INotificationData data)
        {
            if (data is not ExhaustedSleepSignal) return;

            _messageText.text = "You fell asleep from tiredness…";
        }
    }
}