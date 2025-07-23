using UnityEngine;
using System;
using System.Collections.Generic;
using Zenject;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure.Configs.Notifications;

namespace FlavorfulStory.UI.Notifications
{
    /// <summary> Сервис отображения уведомлений. </summary>
    /// <remarks> Показ уведомления зависит от его типа и позиции. </remarks>
    public class NotificationService : MonoBehaviour, INotificationService
    {
        /// <summary> Глобальные настройки системы уведомлений. </summary>
        private NotificationSystemSettings _settings;
        
        /// <summary> Контейнер уведомлений в верхнем левом углу. </summary>
        [SerializeField] private Transform _topLeft;
        /// <summary> Контейнер уведомлений в верхнем правом углу. </summary>
        [SerializeField] private Transform _topRight;
        /// <summary> Контейнер уведомлений в нижнем левом углу. </summary>
        [SerializeField] private Transform _bottomLeft;
        /// <summary> Контейнер уведомлений в нижнем правом углу. </summary>
        [SerializeField] private Transform _bottomRight;
        
        /// <summary> Активные уведомления, сгруппированные по позиции. </summary>
        private readonly Dictionary<NotificationPosition, List<BaseNotificationView>> _activeViewsByPosition = new();
        /// <summary> Кеш конфигураций уведомлений по типу. </summary>
        private readonly Dictionary<NotificationType, NotificationConfig> _configCache = new();

        /// <summary> Инжект глобальных настроек уведомлений. </summary>
        /// <param name="settings"> Настройки системы уведомлений. </param>
        [Inject]
        private void Construct(NotificationSystemSettings settings)
        {
            _settings = settings;
        }
        
        /// <summary> Кеширует конфигурации уведомлений при инициализации. </summary>
        private void Awake()
        {
            foreach (var config in _settings.NotificationConfigs)
                _configCache[config.Type] = config;
        }

        /// <summary> Отображает уведомление по переданным данным. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public void Show(INotificationData data)
        {
            if (!_configCache.TryGetValue(data.Type, out var config)) return;

            var position = config.Position;
            var parent = GetContainer(position);

            var instance = Instantiate(config.Prefab, parent);
            instance.SetupPosition(position, new Vector2(
                _settings.GetHorizontalPadding(position),
                _settings.GetVerticalPadding(position)));
            instance.Initialize(data);
            instance.Show(config.FadeTime, _settings.DefaultEasing);

            if (!_activeViewsByPosition.TryGetValue(position, out var list))
                _activeViewsByPosition[position] = list = new List<BaseNotificationView>();

            list.Add(instance);

            if (position is NotificationPosition.TopLeft or NotificationPosition.TopRight)
                instance.transform.SetAsLastSibling();
            else
                instance.transform.SetAsFirstSibling();

            LifetimeAsync(instance, config).Forget();
            Reposition(list, position);
        }

        /// <summary> Управляет временем жизни уведомления и удалением после завершения. </summary>
        /// <param name="view"> Вьюха уведомления. </param>
        /// <param name="config"> Конфиг уведомления. </param>
        private async UniTaskVoid LifetimeAsync(BaseNotificationView view, NotificationConfig config)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(config.DisplayTime), cancellationToken: this.GetCancellationTokenOnDestroy());

                view.HideAndDestroy(config.FadeTime, _settings.DefaultEasing);
                _activeViewsByPosition[config.Position].Remove(view);
                Reposition(_activeViewsByPosition[config.Position], config.Position);
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary> Перераспределяет позиции уведомлений в указанной зоне. </summary>
        /// <param name="list"> Список активных уведомлений. </param>
        /// <param name="position"> Позиция отображения. </param>
        private void Reposition(List<BaseNotificationView> list, NotificationPosition position)
        {
            float padding = _settings.GetVerticalPadding(position);
            float spacing = _settings.StackSpacing;
            float offsetY = (position is NotificationPosition.TopLeft or NotificationPosition.TopRight) ? -padding : padding;
            bool growDown = position is NotificationPosition.TopLeft or NotificationPosition.TopRight;

            foreach (var view in list)
            {
                float targetY = offsetY;
                view.MoveTo(new Vector2(view.StartXPosition, targetY), _settings.MoveTime, _settings.DefaultEasing);
                offsetY += growDown ? -(view.Height + spacing) : (view.Height + spacing);
            }
        }

        /// <summary> Возвращает соответствующий контейнер для позиции уведомления. </summary>
        /// <param name="position"> Позиция уведомления. </param>
        /// <returns> Трансформ-контейнер для уведомлений. </returns>
        private Transform GetContainer(NotificationPosition position) => position switch
        {
            NotificationPosition.TopLeft     => _topLeft,
            NotificationPosition.TopRight    => _topRight,
            NotificationPosition.BottomLeft  => _bottomLeft,
            NotificationPosition.BottomRight => _bottomRight,
            _ => throw new Exception("Invalid position")
        };
    }
}