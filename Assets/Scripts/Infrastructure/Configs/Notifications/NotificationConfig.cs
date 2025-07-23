using UnityEngine;
using FlavorfulStory.UI.Notifications;

namespace FlavorfulStory.Infrastructure.Configs.Notifications
{
    /// <summary> Конфигурация отображения уведомления конкретного типа. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Notifications/NotificationConfig")]
    public class NotificationConfig : ScriptableObject
    {
        /// <summary> Тип уведомления, которому соответствует данная конфигурация. </summary>
        [field: SerializeField, Tooltip("Тип уведомления")]
        public NotificationType Type { get; private set;}
        
        /// <summary> Позиция на экране, в которой должно отображаться уведомление. </summary>
        [field: SerializeField, Tooltip("Позиция на экране")]
        public NotificationPosition Position { get; private set;}
        
        /// <summary> Префаб UI-элемента уведомления. </summary>
        [field: SerializeField, Tooltip("Префаб уведомления")]
        public BaseNotificationView Prefab { get; private set;}
        
        /// <summary> Время показа уведомления на экране. </summary>
        [field: SerializeField, Tooltip("Время отображения (в секундах)")]
        public float DisplayTime { get; private set;}
        
        /// <summary> Время появления и исчезновения уведомления. </summary>
        [field: SerializeField, Tooltip("Время появления и исчезновения (в секундах)")]
        public float FadeTime { get; private set;}
    }
}