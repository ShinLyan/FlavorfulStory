using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory
{
    [CreateAssetMenu(menuName = "Notifications/SystemSettings")]
    public class NotificationSystemSettings : ScriptableObject
    {
        public float MoveTime = 0.3f;
        //TODO: RectOffset или 4 стороны
        public Vector2 ScreenPadding = new(20, 20);
        public List<NotificationConfig> NotificationConfigs;
    }
}