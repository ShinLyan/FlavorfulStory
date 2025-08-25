using FlavorfulStory.Notifications;

namespace FlavorfulStory.Saving
{
    /// <summary> Сигнал о завершении сохранения игры. </summary>
    public readonly struct SaveCompletedSignal : INotificationData
    {
        /// <summary> Получает тип уведомления. </summary>
        /// <returns> Тип уведомления о сохранении. </returns>
        public NotificationType Type => NotificationType.SaveCompleted;
    }
}