using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory
{
    public class PickupNotificationManager : MonoBehaviour
    {
        [SerializeField] private Transform notificationParent; // Панель, в которую добавляются уведомления
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private float showDuration = 5f;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private int maxNotifications = 3;

        private readonly List<PickupNotification> activeNotifications = new();

        public void ShowNotification(Sprite icon, string text)
        {
            if (activeNotifications.Count >= maxNotifications)
            {
                // Удалить самую верхнюю (старую)
                var toRemove = activeNotifications[0];
                activeNotifications.RemoveAt(0);
                toRemove.FadeAndDestroy(fadeDuration);
            }

            // Сдвинуть остальные вверх
            float offsetY = 0f;
            foreach (var notif in activeNotifications)
            {
                notif.MoveUp(notif.Height, moveDuration);
                offsetY += notif.Height;
            }

            // Создать новую
            var go = Instantiate(notificationPrefab, notificationParent);
            var notification = go.GetComponent<PickupNotification>();
            notification.Initialize(icon, text);
            activeNotifications.Add(notification);

            // Показать с fade
            notification.Show(fadeDuration);

            // Удалить по таймеру
            DOVirtual.DelayedCall(showDuration, () =>
            {
                if (notification != null && activeNotifications.Contains(notification))
                {
                    activeNotifications.Remove(notification);
                    notification.FadeAndDestroy(fadeDuration);
                }
            });
        }
    }
}