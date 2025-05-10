using System.Collections.Generic;
using System.Linq;
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

        //private readonly List<PickupNotification> activeNotifications = new();
        private readonly List<ActiveNotificationData> activeNotifications = new();

        private class ActiveNotificationData
        {
            public PickupNotification Notification;
            public Tween TimerTween;
            public int AccumulatedAmount;
        }
        
        public void ShowNotification(Sprite icon, int amount, string itemName, string itemId)
        {
            var existingData = activeNotifications.FirstOrDefault(n => n.Notification.ItemID == itemId);

            if (existingData != null)
            {
                existingData.AccumulatedAmount += amount;
                existingData.Notification.AddAmount(amount);

                existingData.TimerTween?.Kill(); // Сброс таймера
                existingData.TimerTween = DOVirtual.DelayedCall(showDuration, () =>
                {
                    activeNotifications.Remove(existingData);
                    existingData.Notification.FadeAndDestroy(fadeDuration);
                });

                return;
            }

            if (activeNotifications.Count >= maxNotifications)
            {
                var toRemove = activeNotifications[0];
                activeNotifications.RemoveAt(0);
                toRemove.TimerTween?.Kill();
                toRemove.Notification.FadeAndDestroy(fadeDuration);
            }

            // Сдвинуть остальные
            float offsetY = 0f;
            foreach (var notif in activeNotifications)
                notif.Notification.MoveUp(notif.Notification.Height, moveDuration);

            // Создать новое
            var go = Instantiate(notificationPrefab, notificationParent);
            var notifComponent = go.GetComponent<PickupNotification>();
            notifComponent.Initialize(icon, amount, itemId);

            var data = new ActiveNotificationData
            {
                Notification = notifComponent,
                AccumulatedAmount = amount
            };

            // Создаём таймер отдельно, когда data уже существует
            data.TimerTween = DOVirtual.DelayedCall(showDuration, () =>
            {
                activeNotifications.Remove(data);
                notifComponent.FadeAndDestroy(fadeDuration);
            });

            activeNotifications.Add(data);
            notifComponent.Show(fadeDuration);
        }
    }
}