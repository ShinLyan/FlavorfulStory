using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory
{
    /// <summary> Отвечает за показ уведомлений о подобранных предметах. </summary>
    public class ItemPickupNotificationManager : MonoBehaviour
    {
        /// <summary> Родительский объект для уведомлений. </summary>
        [SerializeField] private Transform _notificationsParent; 
        /// <summary> Префаб уведомления. </summary>
        [SerializeField] private GameObject _notificationPrefab;
        
        /// <summary> Время показа уведомления до его исчезновения. </summary>
        private const float ShowDuration = 5f;
        /// <summary> Длительность анимации появления и исчезновения. </summary>
        private const float FadeDuration = 0.3f;
        /// <summary> Время перемещения уведомлений при перестройке. </summary>
        private const float MoveDuration = 0.3f;
        /// <summary> Максимально допустимое количество активных уведомлений. </summary>
        private const int MaxNotifications = 10;

        /// <summary> Список активных уведомлений на экране. </summary>
        private List<ActiveNotificationData> _activeNotifications;
        /// <summary> Очередь поступающих уведомлений, обрабатываемых в конце кадра. </summary>
        private List<QueuedNotification> _queue;
        /// <summary> Флаг, указывающий на то, что очередь уже обрабатывается. </summary>
        private bool _processingQueued;

        /// <summary> Активное уведомление и его таймер. </summary>
        private class ActiveNotificationData
        {
            public PickupNotification Notification;
            public Tween TimerTween;
            public int AccumulatedAmount;
        }

        /// <summary> Ожидающее уведомление в очереди. </summary>
        private class QueuedNotification
        {
            public Sprite Icon;
            public string ItemName;
            public string ItemID;
            public int Amount;
        }

        /// <summary> Инициализация списков при старте. </summary>
        private void Awake()
        {
            _activeNotifications = new();
            _queue = new();
        }
        
        /// <summary> Добавляет уведомление в очередь (обработка — в конце кадра). </summary>
        public void ShowNotification(Sprite icon, int amount, string itemName, string itemId)
        {
            _queue.Add(new QueuedNotification
            {
                Icon = icon,
                ItemID = itemId,
                ItemName = itemName,
                Amount = amount
            });

            if (!_processingQueued)
            {
                _processingQueued = true;
                StartCoroutine(ProcessQueuedNextFrame());
            }
        }

        /// <summary> Группирует очередь по ID и запускает показ уведомлений. </summary>
        private IEnumerator ProcessQueuedNextFrame()
        {
            yield return new WaitForEndOfFrame();

            var grouped = _queue
                .GroupBy(q => q.ItemID)
                .Select(g =>
                {
                    var first = g.First();
                    return new QueuedNotification
                    {
                        Icon = first.Icon,
                        ItemID = g.Key,
                        ItemName = first.ItemName,
                        Amount = g.Sum(x => x.Amount)
                    };
                });

            foreach (var notification in grouped)
            {
                ProcessNotification(notification.Icon, notification.Amount, notification.ItemName, notification.ItemID);
            }

            _queue.Clear();
            _processingQueued = false;
        }

        /// <summary> Обрабатывает показ или обновление уведомления. </summary>
        private void ProcessNotification(Sprite icon, int amount, string itemName, string itemId)
        {
            if (TryUpdateExisting(itemId, amount)) return;

            if (_activeNotifications.Count >= MaxNotifications)
                RemoveOldestNotification();

            CreateNewNotification(icon, amount, itemName, itemId);
        }

        /// <summary> Пробует обновить уже показанное уведомление. </summary>
        private bool TryUpdateExisting(string itemId, int amount)
        {
            var existing = _activeNotifications.FirstOrDefault(n => n.Notification.ItemID == itemId);
            if (existing == null) return false;

            existing.AccumulatedAmount += amount;
            existing.Notification.AddAmount(amount);

            ResetLifetime(existing);

            return true;
        }

        /// <summary> Создаёт новое уведомление и запускает его отображение. </summary>
        private void CreateNewNotification(Sprite icon, int amount, string itemName, string itemId)
        {
            var go = Instantiate(_notificationPrefab, _notificationsParent);
            var notif = go.GetComponent<PickupNotification>();
            notif.Initialize(icon, amount, itemId, itemName);

            var data = new ActiveNotificationData
            {
                Notification = notif,
                AccumulatedAmount = amount
            };

            _activeNotifications.Add(data);
            notif.Show(FadeDuration);

            ResetLifetime(data);
            RepositionNotifications();
        }

        /// <summary> Сбрасывает таймер жизни уведомления. </summary>
        private void ResetLifetime(ActiveNotificationData data)
        {
            data.TimerTween?.Kill(true);
            data.TimerTween = DOVirtual.DelayedCall(ShowDuration, () =>
            {
                _activeNotifications.Remove(data);
                data.Notification.FadeAndDestroy(FadeDuration);
                RepositionNotifications();
            });
        }

        /// <summary> Удаляет самое старое уведомление. </summary>
        private void RemoveOldestNotification()
        {
            var oldest = _activeNotifications[0];
            _activeNotifications.RemoveAt(0);
            oldest.TimerTween?.Kill(true);
            oldest.Notification.FadeAndDestroy(FadeDuration);
        }

        /// <summary> Переупорядочивает активные уведомления по вертикали. </summary>
        private void RepositionNotifications()
        {
            float totalHeight = 0f;

            foreach (var data in _activeNotifications)
            {
                var notif = data.Notification;
                float targetY = totalHeight;
                var currentPos = notif.RectTransform.anchoredPosition;
                if (Mathf.Abs(currentPos.y - targetY) > 0.01f)
                {
                    notif.SetPosition(new Vector2(notif.GetStartX(), targetY), MoveDuration);
                }
                totalHeight += notif.Height;
            }
        }
    }
}