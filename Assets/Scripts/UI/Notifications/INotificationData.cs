namespace FlavorfulStory.UI.Notifications
{
    /// <summary> Интерфейс данных уведомления, содержащий его тип. </summary>
    public interface INotificationData
    {
        /// <summary> Тип уведомления, используемый для определения конфигурации и отображения. </summary>
        NotificationType Type { get; }
    }
}