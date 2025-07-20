using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory
{
    public class NotificationService : MonoBehaviour, INotificationService
    {
        [SerializeField] private NotificationSystemSettings _settings;
        [SerializeField] private Transform _topLeft;
        [SerializeField] private Transform _topRight;
        [SerializeField] private Transform _bottomLeft;
        [SerializeField] private Transform _bottomRight;

        private readonly Dictionary<NotificationType, List<BaseNotificationView>> _active = new();
        private readonly Dictionary<NotificationType, NotificationConfig> _configCache = new();

        private void Awake()
        {
            foreach (var cfg in _settings.NotificationConfigs)
                _configCache[cfg.Type] = cfg;
        }

        public void Show(INotificationData data)
        {
            if (!_configCache.TryGetValue(data.Type, out var config)) return;

            var parent = GetContainer(config.Position);
            var instance = Instantiate(config.Prefab, parent);
            instance.SetupPosition(config.Position, _settings.ScreenPadding);
            instance.Initialize(data);
            instance.Show(config.FadeTime);

            if (!_active.TryGetValue(data.Type, out var list))
                _active[data.Type] = list = new List<BaseNotificationView>();

            list.Add(instance);

            if (config.Position is NotificationPosition.TopLeft or NotificationPosition.TopRight)
                instance.transform.SetAsLastSibling(); // сверху вниз
            else
                instance.transform.SetAsFirstSibling(); // снизу вверх

            StartCoroutine(Lifetime(instance, config));
            Reposition(list, config.Position);
        }

        private IEnumerator Lifetime(BaseNotificationView view, NotificationConfig cfg)
        {
            yield return new WaitForSeconds(cfg.DisplayTime);
            view.HideAndDestroy(cfg.FadeTime);
            _active[cfg.Type].Remove(view);
            Reposition(_active[cfg.Type], cfg.Position);
        }

        private void Reposition(List<BaseNotificationView> list, NotificationPosition pos)
        {
            Vector2 padding = _settings.ScreenPadding;
            float offsetY = (pos is NotificationPosition.TopLeft or NotificationPosition.TopRight) ? -padding.y : padding.y;
            bool growDown = pos is NotificationPosition.TopLeft or NotificationPosition.TopRight;

            foreach (var view in list)
            {
                float targetY = offsetY;
                view.MoveTo(new Vector2(view.StartXPosition, targetY), _settings.MoveTime);
                offsetY += growDown ? -view.Height : view.Height;
            }
        }

        private Transform GetContainer(NotificationPosition pos) => pos switch
        {
            NotificationPosition.TopLeft     => _topLeft,
            NotificationPosition.TopRight    => _topRight,
            NotificationPosition.BottomLeft  => _bottomLeft,
            NotificationPosition.BottomRight => _bottomRight,
            _ => throw new System.Exception("Invalid position")
        };
    }
}