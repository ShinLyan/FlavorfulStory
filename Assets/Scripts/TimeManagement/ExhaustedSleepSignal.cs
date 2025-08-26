using FlavorfulStory.Notifications;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Сигнал, отправляемый при наступлении ночи. </summary>
    public readonly struct ExhaustedSleepSignal : INotificationData
    {
        /// <summary> Получает тип уведомления. </summary>
        /// <returns> Тип уведомления о сне от истощения. </returns>
        public NotificationType Type => NotificationType.ExhaustedSleep;
    }
}