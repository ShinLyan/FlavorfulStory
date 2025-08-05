namespace FlavorfulStory.Notifications.Data
{
    /// <summary> Данные уведомления о добавлении нового квеста. </summary>
    public class QuestAddedNotificationData : INotificationData
    {
        /// <summary> Тип уведомления: "Квест добавлен". </summary>
        public NotificationType Type => NotificationType.QuestAdded;

        /// <summary> Название добавленного квеста. </summary>
        public string QuestName { get; }

        /// <summary> Конструктор, инициализирующий название квеста. </summary>
        /// <param name="questName"> Название нового квеста. </param>
        public QuestAddedNotificationData(string questName) => QuestName = questName;
    }
}