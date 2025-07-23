using UnityEngine;
using FlavorfulStory.UI.Notifications;

namespace FlavorfulStory.Infrastructure.Configs.Notifications
{
    /// <summary> Конфигурация отображения уведомления конкретного типа. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Notifications/NotificationConfig")]
    public class NotificationConfig : ScriptableObject
    {
        /// <summary> Тип уведомления, которому соответствует данная конфигурация. </summary>
        public NotificationType Type;
        /// <summary> Позиция на экране, в которой должно отображаться уведомление. </summary>
        public NotificationPosition Position;
        /// <summary> Префаб UI-элемента уведомления. </summary>
        public BaseNotificationView Prefab;
        /// <summary> Время показа уведомления на экране. </summary>
        public float DisplayTime;
        /// <summary> Время появления и исчезновения уведомления. </summary>
        public float FadeTime;
    }
}