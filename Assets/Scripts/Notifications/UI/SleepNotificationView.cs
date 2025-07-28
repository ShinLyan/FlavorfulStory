namespace FlavorfulStory.Notifications.UI
{
    /// <summary> UI-элемент уведомления о наступлении ночи. </summary>
    public class SleepNotificationView : BaseNotificationView
    {
        /// <summary> Инициализирует уведомление данными о ночи. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not SleepNotificationData notificationData) return;

            _label.text = $"Скоро наступит ночь. Осталось {(int)notificationData.Hour} часов.";
        }
    }
}