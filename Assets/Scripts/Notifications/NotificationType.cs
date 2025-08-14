namespace FlavorfulStory.Notifications
{
    /// <summary> Тип уведомления, определяющий его поведение и визуальное представление. </summary>
    public enum NotificationType
    {
        /// <summary> Уведомление о подобранном предмете. </summary>
        ItemCollected,

        /// <summary> Уведомление о наступлении ночи. </summary>
        NightStarted,

        /// <summary> Уведомление о добавлении нового квеста. </summary>
        QuestAdded,

        /// <summary> Уведомление о невозможности разрушить объект. </summary>
        DismantleDenied
    }
}