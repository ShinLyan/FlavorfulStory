using FlavorfulStory.Notifications.UI;
using UnityEngine;

namespace FlavorfulStory.Notifications.Configs
{
    /// <summary> Конфигурация отображения уведомления конкретного типа. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/UI/Notifications/NotificationConfig")]
    public class NotificationConfig : ScriptableObject
    {
        /// <summary> Тип уведомления, которому соответствует данная конфигурация. </summary>
        [field: SerializeField, Tooltip("Тип уведомления.")]
        public NotificationType Type { get; private set; }

        /// <summary> Позиция на экране, в которой должно отображаться уведомление. </summary>
        [field: SerializeField, Tooltip("Позиция на экране.")]
        public NotificationPosition Position { get; private set; }

        /// <summary> Префаб уведомления. </summary>
        [field: SerializeField, Tooltip("Префаб уведомления.")]
        public BaseNotificationView Prefab { get; private set; }
    }
}