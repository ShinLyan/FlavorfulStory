using UnityEngine;
using TMPro;

namespace FlavorfulStory.UI.Notifications
{
    /// <summary> UI-элемент уведомления о наступлении ночи. </summary>
    public class SleepNotificationView : BaseNotificationView
    {
        /// <summary> Текстовое поле с сообщением о сне. </summary>
        [SerializeField] private TMP_Text _label;

        /// <summary> Высота уведомления, рассчитываемая по RectTransform. </summary>
        public override float Height => RectTransform.rect.height;

        /// <summary> Инициализирует уведомление данными о ночи. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public override void Initialize(INotificationData data)
        {
            if (data is not SleepNotificationData d) return;
            _label.text = $"Дирк ждет тебя в кроватки! Уже наступила ночь.";
        }
    }
}