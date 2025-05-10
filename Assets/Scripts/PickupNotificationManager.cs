using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory
{
    public class PickupNotificationManager : MonoBehaviour
    {
        [SerializeField] private Transform notificationParent;
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private float showDuration = 5f;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private int maxNotifications = 5;

        private readonly List<ActiveNotificationData> activeNotifications = new();

        private class ActiveNotificationData
        {
            public PickupNotification Notification;
            public Tween TimerTween;
            public int AccumulatedAmount;
        }

        private class QueuedNotification
        {
            public Sprite Icon;
            public string ItemName;
            public string ItemID;
            public int Amount;
        }

        private readonly List<QueuedNotification> queue = new();
        private bool processingQueued;

        /// <summary>
        /// Вызывается при подборе предмета. Буферизуется и обрабатывается в конце кадра.
        /// </summary>
        public void ShowNotification(Sprite icon, int amount, string itemName, string itemId)
        {
            queue.Add(new QueuedNotification
            {
                Icon = icon,
                ItemID = itemId,
                ItemName = itemName,
                Amount = amount
            });

            if (!processingQueued)
            {
                processingQueued = true;
                StartCoroutine(ProcessQueuedNextFrame());
            }
        }

        private IEnumerator ProcessQueuedNextFrame()
        {
            yield return new WaitForEndOfFrame();

            var grouped = queue
                .GroupBy(q => q.ItemID)
                .Select(g =>
                {
                    var combined = g.First();
                    combined.Amount = g.Sum(x => x.Amount);
                    return combined;
                });

            foreach (var notification in grouped)
            {
                ProcessNotification(notification.Icon, notification.Amount, notification.ItemName, notification.ItemID);
            }

            queue.Clear();
            processingQueued = false;
        }

        private void ProcessNotification(Sprite icon, int amount, string itemName, string itemId)
        {
            var existingData = activeNotifications.FirstOrDefault(n => n.Notification.ItemID == itemId);

            if (existingData != null)
            {
                existingData.AccumulatedAmount += amount;
                existingData.Notification.AddAmount(amount);

                existingData.TimerTween?.Kill();
                existingData.TimerTween = DOVirtual.DelayedCall(showDuration, () =>
                {
                    activeNotifications.Remove(existingData);
                    existingData.Notification.FadeAndDestroy(fadeDuration);
                    RepositionNotifications();
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

            var go = Instantiate(notificationPrefab, notificationParent);
            var notifComponent = go.GetComponent<PickupNotification>();
            notifComponent.Initialize(icon, amount, itemId, itemName);

            var data = new ActiveNotificationData
            {
                Notification = notifComponent,
                AccumulatedAmount = amount
            };

            data.TimerTween = DOVirtual.DelayedCall(showDuration, () =>
            {
                activeNotifications.Remove(data);
                notifComponent.FadeAndDestroy(fadeDuration);
                RepositionNotifications();
            });

            activeNotifications.Add(data);
            notifComponent.Show(fadeDuration);

            RepositionNotifications();
        }

        private void RepositionNotifications()
        {
            float totalHeight = 0f;

            for (int i = 0; i < activeNotifications.Count; i++) // Сверху вниз
            {
                var notification = activeNotifications[i].Notification;
                float targetY = totalHeight;
                var anchoredPosition = new Vector2(notification.GetStartX(), targetY);
                notification.SetPosition(anchoredPosition, moveDuration);
                totalHeight += notification.Height;
            }
        }
    }
}