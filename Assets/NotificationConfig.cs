using UnityEngine;

namespace FlavorfulStory
{
    [CreateAssetMenu(menuName = "Notifications/NotificationConfig")]
    public class NotificationConfig : ScriptableObject
    {
        public NotificationType Type;
        public NotificationPosition Position;
        public BaseNotificationView Prefab;
        public float DisplayTime = 4f;
        public float FadeTime = 0.3f;
    }
}