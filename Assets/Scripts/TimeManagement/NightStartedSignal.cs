using FlavorfulStory.Notifications;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Сигнал, отправляемый при наступлении ночи. </summary>
    public struct NightStartedSignal : INotificationData
    {
        /// <summary> Тип уведомления. </summary>
        public NotificationType Type => NotificationType.NightStarted;

        /// <summary> Внутриигровое время, когда наступила ночь. </summary>
        public DateTime Time { get; }

        /// <summary> Создаёт новый сигнал с указанием времени. </summary>
        /// <param name="time"> Текущее внутриигровое время. </param>
        public NightStartedSignal(DateTime time) => Time = time;
    }
}