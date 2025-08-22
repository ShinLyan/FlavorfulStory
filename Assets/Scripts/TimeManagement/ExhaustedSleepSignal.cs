using FlavorfulStory.Notifications;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Сигнал, отправляемый при наступлении ночи. </summary>
    public struct ExhaustedSleepSignal : INotificationData
    {
        /// <summary> Тип уведомления. </summary>
        public NotificationType Type => NotificationType.ExhaustedSleep;
    }
}