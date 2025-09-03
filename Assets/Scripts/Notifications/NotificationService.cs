using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure;
using FlavorfulStory.Notifications.Configs;
using FlavorfulStory.Notifications.UI;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace FlavorfulStory.Notifications
{
    /// <summary> Сервис отображения уведомлений с поддержкой ObjectPool. </summary>
    /// <remarks> Показ уведомления зависит от его типа и позиции. </remarks>
    public class NotificationService : IInitializable, IDisposable, INotificationService
    {
        /// <summary> Глобальные настройки системы уведомлений. </summary>
        private readonly NotificationsSettings _settings;

        /// <summary> Компонент, содержащий якоря для разных зон экрана. </summary>
        private readonly NotificationAnchorLocator _locator;

        /// <summary> Кеш конфигураций уведомлений по типу. </summary>
        private readonly Dictionary<NotificationType, NotificationConfig> _configCache;

        /// <summary> Активные уведомления, сгруппированные по позиции. </summary>
        private readonly Dictionary<NotificationPosition, List<BaseNotificationView>> _activeViewsByPosition;

        /// <summary> Пулы вьюшек для каждого типа уведомлений. </summary>
        private readonly Dictionary<NotificationType, ObjectPool<BaseNotificationView>> _pools;

        /// <summary> Конструктор сервиса уведомлений. </summary>
        /// <param name="settings"> Глобальные настройки отображения. </param>
        /// <param name="locator"> Компонент с привязками к позициям на экране. </param>
        public NotificationService(NotificationsSettings settings, NotificationAnchorLocator locator)
        {
            _settings = settings;
            _locator = locator;
            _configCache = new Dictionary<NotificationType, NotificationConfig>();
            _activeViewsByPosition = new Dictionary<NotificationPosition, List<BaseNotificationView>>();
            _pools = new Dictionary<NotificationType, ObjectPool<BaseNotificationView>>();
        }

        /// <summary> Кеширует конфигурации уведомлений при инициализации. </summary>
        public void Initialize()
        {
            InitConfigCache();
            InitPools();
        }

        /// <summary> Заполняет словарь конфигураций уведомлений по типам. </summary>
        private void InitConfigCache()
        {
            foreach (var config in _settings.NotificationConfigs) _configCache[config.Type] = config;
        }

        /// <summary> Создаёт ObjectPool для каждого типа уведомлений. </summary>
        private void InitPools()
        {
            foreach (var config in _settings.NotificationConfigs)
            {
                var prefab = config.Prefab;
                var pool = new ObjectPool<BaseNotificationView>(
                    () => Object.Instantiate(prefab, _locator.GetContainer(config.Position)),
                    view => view.gameObject.SetActive(true),
                    view => view.gameObject.SetActive(false)
                );

                _pools[config.Type] = pool;
            }
        }

        /// <summary> Очищает все пулы при уничтожении. </summary>
        public void Dispose()
        {
            foreach (var pool in _pools.Values) pool.Clear();
        }

        /// <summary> Отображает уведомление по переданным данным. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public void Show(INotificationData data)
        {
            if (!_configCache.TryGetValue(data.Type, out var config)) return;

            var position = config.Position;
            var pool = _pools[config.Type];
            var view = pool.Get();

            view.Initialize(data);

            if (!_activeViewsByPosition.TryGetValue(position, out var list))
                _activeViewsByPosition[position] = list = new List<BaseNotificationView>();

            list.Add(view);

            view.SetupPosition(position,
                new Vector2(_settings.GetHorizontalPadding(position), _settings.GetVerticalPadding(position)));
            Reposition(list, position);

            view.Show(_settings.FadeTime, _settings.DefaultEasing);
            view.transform.SetAsLastSibling();

            HandleLifetime(view, config, pool).Forget();
        }

        /// <summary> Управляет временем жизни уведомления и удалением после завершения. </summary>
        /// <param name="view"> Отображение уведомления. </param>
        /// <param name="config"> Конфигурация уведомления. </param>
        /// <param name="pool"> Пул, в который возвращается отображение после завершения. </param>
        private async UniTaskVoid HandleLifetime(BaseNotificationView view, NotificationConfig config,
            ObjectPool<BaseNotificationView> pool)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_settings.DisplayTime));

            view.Hide(_settings.FadeTime, _settings.DefaultEasing);

            await UniTask.Delay(TimeSpan.FromSeconds(_settings.FadeTime));

            _activeViewsByPosition[config.Position].Remove(view);
            Reposition(_activeViewsByPosition[config.Position], config.Position);
            pool.Release(view);
        }

        /// <summary> Перераспределяет позиции уведомлений в указанной зоне. </summary>
        /// <param name="list"> Список активных уведомлений. </param>
        /// <param name="position"> Позиция отображения. </param>
        private void Reposition(List<BaseNotificationView> list, NotificationPosition position)
        {
            float padding = _settings.GetVerticalPadding(position);
            float spacing = _settings.StackSpacing;
            float offsetY = position is NotificationPosition.TopLeft or NotificationPosition.TopRight
                ? -padding
                : padding;
            bool growDown = position is NotificationPosition.TopLeft or NotificationPosition.TopRight;

            foreach (var view in list)
            {
                view.MoveTo(new Vector2(view.StartXPosition, offsetY), _settings.MoveTime, _settings.DefaultEasing);
                offsetY += growDown ? -(view.Height + spacing) : view.Height + spacing;
            }
        }
    }
}