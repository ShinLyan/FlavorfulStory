using UnityEngine;
using TMPro;

namespace FlavorfulStory.UI.Notifications
{
    public class SleepNotificationView : BaseNotificationView
    {
        [SerializeField] private TMP_Text _label;

        public override float Height => RectTransform.rect.height;

        public override void Initialize(INotificationData data)
        {
            if (data is not SleepNotificationData d) return;
            _label.text = $"Дирк ждет тебя в кроватки! Уже наступила ночь.";
        }
    }
}