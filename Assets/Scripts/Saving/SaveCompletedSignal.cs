using FlavorfulStory.Notifications;

namespace FlavorfulStory.Saving
{
    public class SaveCompletedSignal : INotificationData
    {
        public NotificationType Type => NotificationType.SaveCompleted;
    }
}