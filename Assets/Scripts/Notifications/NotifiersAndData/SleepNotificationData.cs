namespace FlavorfulStory.Notifications
{
    /// <summary> Данные для уведомления о наступлении ночи. </summary>
    public class SleepNotificationData : INotificationData
    {
        /// <summary> Тип уведомления. </summary>
        public NotificationType Type => NotificationType.Sleep;

        /// <summary> Час, в который наступила ночь. </summary>
        public float Hour { get; }

        /// <summary> Конструктор с параметрами. </summary>
        /// <param name="hour"> Час, в который наступила ночь. </param>
        public SleepNotificationData(float hour) => Hour = hour;
    }
}