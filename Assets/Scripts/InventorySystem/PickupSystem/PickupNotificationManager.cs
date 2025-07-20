using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Отвечает за показ уведомлений о подобранных предметах. </summary>
    public class PickupNotificationManager : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> Родительский объект для уведомлений. </summary>
        [SerializeField] private Transform _notificationsParent;

        /// <summary> Префаб уведомления. </summary>
        [SerializeField] private GameObject _notificationPrefab;

        /// <summary> Список активных уведомлений на экране. </summary>
        private List<ActiveNotification> _activeNotifications;

        /// <summary> Очередь поступающих уведомлений, обрабатываемых в конце кадра. </summary>
        private List<QueuedNotification> _queuedNotifications;

        /// <summary> Очередь уже обрабатывается? </summary>
        private bool _isQueueProcessing;

        /// <summary> Время показа уведомления до его исчезновения. </summary>
        private const float DisplayDuration = 5f;

        /// <summary> Длительность анимации появления и исчезновения. </summary>
        private const float FadeDuration = 0.3f;

        /// <summary> Время перемещения уведомлений при перестройке. </summary>
        private const float MoveDuration = 0.3f;

        /// <summary> Максимально допустимое количество активных уведомлений. </summary>
        private const int MaxActiveNotifications = 10;

        /// <summary> Активное уведомление и его таймер. </summary>
        private class ActiveNotification
        {
            /// <summary> Визуальное представление уведомления. </summary>
            public BaseNotificationView View;

            /// <summary> Таймер жизни уведомления. </summary>
            public Tween LifetimeTween;
        }

        /// <summary> Ожидающее уведомление в очереди. </summary>
        private class QueuedNotification
        {
            /// <summary> Иконка предмета. </summary>
            public Sprite Icon;

            /// <summary> Название предмета. </summary>
            public string ItemName;

            /// <summary> Идентификатор предмета. </summary>
            public string ItemID;

            /// <summary> Количество предметов. </summary>
            public int Amount;
        }

        #endregion

        /// <summary> Инициализация списков при старте. </summary>
        private void Awake()
        {
            _activeNotifications = new List<ActiveNotification>();
            _queuedNotifications = new List<QueuedNotification>();
        }

        /// <summary> Добавить уведомление в очередь (обрабатывается в конце кадра). </summary>
        /// <param name="icon"> Иконка предмета. </param>
        /// <param name="amount"> Количество. </param>
        /// <param name="itemName"> Название предмета. </param>
        /// <param name="itemId"> Идентификатор предмета. </param>
        public void ShowNotification(Sprite icon, int amount, string itemName, string itemId)
        {
            _queuedNotifications.Add(new QueuedNotification
            {
                Icon = icon,
                ItemID = itemId,
                ItemName = itemName,
                Amount = amount
            });

            if (!_isQueueProcessing)
            {
                _isQueueProcessing = true;
                StartCoroutine(ProcessNotificationQueue());
            }
        }

        /// <summary> Обработка очереди уведомлений с агрегацией по ID предметов. </summary>
        /// <returns> Корутина ожидания конца кадра и показа уведомлений. </returns>
        private IEnumerator ProcessNotificationQueue()
        {
            yield return new WaitForEndOfFrame();

            var groupedNotifications = _queuedNotifications
                .GroupBy(notification => notification.ItemID)
                .Select(group =>
                {
                    var firstNotification = group.First();
                    return new QueuedNotification
                    {
                        Icon = firstNotification.Icon,
                        ItemID = group.Key,
                        ItemName = firstNotification.ItemName,
                        Amount = group.Sum(notification => notification.Amount)
                    };
                });

            foreach (var notification in groupedNotifications)
                HandleNotification(notification.Icon, notification.Amount, notification.ItemName, notification.ItemID);

            _queuedNotifications.Clear();
            _isQueueProcessing = false;
        }

        /// <summary> Обработать новое или обновить существующее уведомление. </summary>
        /// <param name="icon"> Иконка предмета. </param>
        /// <param name="amount"> Количество предметов. </param>
        /// <param name="itemName"> Название предмета. </param>
        /// <param name="itemId"> Уникальный идентификатор предмета. </param>
        private void HandleNotification(Sprite icon, int amount, string itemName, string itemId)
        {
            if (TryUpdateExistingNotification(itemId, amount)) return;

            if (_activeNotifications.Count >= MaxActiveNotifications)
                HideAndRemoveNotification(_activeNotifications[0]);

            CreateNewNotification(icon, amount, itemName, itemId);
        }

        /// <summary> Проверить и обновить уже отображаемое уведомление. </summary>
        /// <param name="itemId"> Идентификатор предмета. </param>
        /// <param name="amount"> Количество добавляемых предметов. </param>
        /// <returns> true — если уведомление было найдено и обновлено; иначе false. </returns>
        private bool TryUpdateExistingNotification(string itemId, int amount)
        {
            // var existingNotification =
            //     _activeNotifications.FirstOrDefault(notification => notification.View.ItemID == itemId);
            // if (existingNotification == null) return false;
            //
            // existingNotification.View.AddAmount(amount);
            // ResetNotificationLifetime(existingNotification);

            return true;
        }

        /// <summary> Создать и отобразить новое уведомление. </summary>
        /// <param name="icon"> Иконка предмета. </param>
        /// <param name="amount"> Количество предметов. </param>
        /// <param name="itemName"> Название предмета. </param>
        /// <param name="itemId"> Уникальный идентификатор предмета. </param>
        private void CreateNewNotification(Sprite icon, int amount, string itemName, string itemId)
        {
            var instance = Instantiate(_notificationPrefab, _notificationsParent);
            var view = instance.GetComponent<PickupNotificationView>();
            
            // view.SetPositionInfo(NotificationPosition.TopLeft);
            // view.Initialize(icon, amount, itemId, itemName);

            var notification = new ActiveNotification { View = view };

            _activeNotifications.Add(notification);
            view.Show(FadeDuration);

            ResetNotificationLifetime(notification);
            RepositionNotifications();
        }

        /// <summary> Сбросить таймер жизни уведомления и установить новый. </summary>
        /// <param name="notification"> Активное уведомление. </param>
        private void ResetNotificationLifetime(ActiveNotification notification)
        {
            notification.LifetimeTween = DOVirtual.DelayedCall(DisplayDuration, () =>
            {
                HideAndRemoveNotification(notification);
                RepositionNotifications();
            });
        }

        /// <summary> Скрыть уведомление с анимацией и удалить его из списка. </summary>
        /// <param name="notification"> Уведомление для удаления. </param>
        private void HideAndRemoveNotification(ActiveNotification notification)
        {
            _activeNotifications.Remove(notification);
            notification.LifetimeTween?.Kill(true);
            //notification.View.FadeAndDestroy(FadeDuration);
        }

        /// <summary> Переупорядочить активные уведомления по вертикали с анимацией. </summary>
        private void RepositionNotifications()
        {
            float totalOffset = 0f;
            bool growDownward = _activeNotifications.FirstOrDefault()?.View.Position is NotificationPosition.TopLeft or NotificationPosition.TopRight;

            foreach (var notification in _activeNotifications)
            {
                var view = notification.View;
                float targetY = growDownward ? -totalOffset : totalOffset;

                if (!Mathf.Approximately(view.RectTransform.anchoredPosition.y, targetY))
                {
                    var newPosition = new Vector2(view.StartXPosition, targetY);
                    // view.SetPosition(newPosition, MoveDuration);
                }

                totalOffset += view.Height;
            }
        }
    }
}