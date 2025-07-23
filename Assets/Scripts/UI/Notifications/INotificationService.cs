namespace FlavorfulStory.UI.Notifications
{
    /// <summary> Интерфейс сервиса отображения уведомлений. </summary>
    public interface INotificationService
    {
        /// <summary> Отображает уведомление на экране. </summary>
        /// <param name="data"> Данные уведомления. </param>
        void Show(INotificationData data);
    }
}