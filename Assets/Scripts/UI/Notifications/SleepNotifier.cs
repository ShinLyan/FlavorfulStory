using UnityEngine;
using Zenject;
using FlavorfulStory.TimeManagement;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.UI.Notifications
{
    /// <summary> Отправляет уведомление при наступлении ночи. </summary>
    public class SleepNotifier : MonoBehaviour
    {
        /// <summary> Сервис отображения уведомлений. </summary>
        private INotificationService _notificationService;
        
        /// <summary> Инжект зависимости сервиса уведомлений. </summary>
        /// <param name="notificationService"> Сервис уведомлений. </param>
        [Inject]
        private void Construct(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary> Подписка на событие начала ночи. </summary>
        private void OnEnable() => WorldTime.OnNightStarted += NightStartedHandler; 
        
        /// <summary> Отписка от события начала ночи. </summary>
        private void OnDisable() => WorldTime.OnNightStarted -= NightStartedHandler;

        /// <summary> Обработчик события начала ночи. </summary>
        /// <param name="dateTime"> Текущее внутриигровое время. </param>
        private void NightStartedHandler(DateTime dateTime)
        {
            _notificationService.Show(new SleepNotificationData()
            {
                hour = (int) dateTime.Hour
            });
        }
    }
}