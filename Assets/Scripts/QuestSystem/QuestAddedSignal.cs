using FlavorfulStory.Notifications;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Сигнал, отправляемый при добавлении нового квеста. </summary>
    public struct QuestAddedSignal : INotificationData
    {
        /// <summary> Тип уведомления. </summary>
        public NotificationType Type => NotificationType.QuestAdded;

        /// <summary> Название добавленного квеста. </summary>
        public string QuestName { get; }

        /// <summary> Конструктор сигнала с названием квеста. </summary>
        /// <param name="questName"> Название нового квеста. </param>
        public QuestAddedSignal(string questName) => QuestName = questName;
    }
}