namespace FlavorfulStory.Notifications
{
    /// <summary> Данные для уведомления о наступлении ночи. </summary>
    public struct SleepNotificationData : INotificationData
    {
        /// <summary> Тип уведомления. </summary>
        public NotificationType Type => NotificationType.Sleep;

        /// <summary> Час, в который наступила ночь. </summary>
        public float Hour { get; }

        public SleepNotificationData(float hour) => Hour = hour;
    }
}