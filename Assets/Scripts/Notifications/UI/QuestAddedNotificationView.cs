using FlavorfulStory.QuestSystem;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> Представление уведомления о добавлении нового квеста. </summary>
    public class QuestAddedNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле для отображения названия квеста. </summary>
        [SerializeField] private TMP_Text _questNameText;

        /// <summary> Инициализирует содержимое уведомления на основе переданных данных. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not QuestAddedSignal notificationData) return;

            _questNameText.text = notificationData.QuestName;
        }
    }
}