using FlavorfulStory.Notifications;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Сигнал, отправляемый когда проигрался принудительный сон. </summary>
    public readonly struct ExhaustedSleepSignal : INotificationData
    {
        /// <summary> Получает тип уведомления. </summary>
        /// <returns> Тип уведомления о сне от истощения. </returns>
        public NotificationType Type => NotificationType.ExhaustedSleep;
    }
}