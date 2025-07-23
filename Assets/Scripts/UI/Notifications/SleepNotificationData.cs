namespace FlavorfulStory.UI.Notifications
{
    /// <summary> Данные для уведомления о наступлении ночи. </summary>
    public class SleepNotificationData  : INotificationData
    {
        /// <summary> Тип уведомления. </summary>
        public NotificationType Type => NotificationType.Sleep;
        
        /// <summary> Час, в который наступила ночь. </summary>
        public int hour;
    }
}