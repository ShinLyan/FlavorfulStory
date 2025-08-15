using FlavorfulStory.Notifications;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Сигнал уведомления о невозможности демонтажа объекта. </summary>
    public readonly struct DismantleDeniedSignal : INotificationData
    {
        /// <summary> Сообщение, объясняющее причину отказа. </summary>
        public string Message { get; }

        /// <summary> Тип уведомления (используется системой оповещений). </summary>
        public NotificationType Type => NotificationType.DismantleDenied;

        /// <summary> Конструктор сигнала отказа демонтажа. </summary>
        /// <param name="message"> Текст сообщения для отображения. </param>
        public DismantleDeniedSignal(string message) => Message = message;
    }
}