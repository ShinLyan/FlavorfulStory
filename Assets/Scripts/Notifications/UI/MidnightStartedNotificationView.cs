using FlavorfulStory.TimeManagement;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> Отображение уведомления о наступлении полуночи. </summary>
    public class MidnightStartedNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле с сообщением. </summary>
        [SerializeField] private TMP_Text _messageText;

        /// <summary> Инициализирует уведомление данными о полуночи. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not MidnightStartedSignal) return;

            _messageText.text = "It’s getting pretty late. Time for bed"; //TODO: localize
        }
    }
}