using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.Notifications.Configs
{
    /// <summary> Глобальные настройки системы уведомлений. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/UI/Notifications/NotificationsSettings")]
    public class NotificationsSettings : ScriptableObject
    {
        /// <summary> Время перемещения уведомлений при перестроении. </summary>
        [field: SerializeField, Tooltip("Время перемещения уведомлений.")]
        public float MoveTime { get; private set; }

        /// <summary> Расстояние между уведомлениями в стеке. </summary>
        [field: SerializeField, Tooltip("Расстояние между уведомлениями.")]
        public float StackSpacing { get; private set; }

        /// <summary> Стандартная функция easing для анимаций. </summary>
        [field: SerializeField, Tooltip("Стандартная функция easing.")]
        public Ease DefaultEasing { get; private set; }

        /// <summary> Отступ сверху экрана для уведомлений. </summary>
        [Header("Screen Padding")]
        [field: SerializeField, Tooltip("Отступ сверху экрана.")]
        public float PaddingTop { get; private set; }

        /// <summary> Отступ снизу экрана для уведомлений. </summary>
        [field: SerializeField, Tooltip("Отступ снизу экрана.")]
        public float PaddingBottom { get; private set; }

        /// <summary> Отступ слева экрана для уведомлений. </summary>
        [field: SerializeField, Tooltip("Отступ слева экрана.")]
        public float PaddingLeft { get; private set; }

        /// <summary> Отступ справа экрана для уведомлений. </summary>
        [field: SerializeField, Tooltip("Отступ справа экрана.")]
        public float PaddingRight { get; private set; }

        /// <summary> Список конфигураций всех типов уведомлений. </summary>
        [field: SerializeField, Tooltip("Конфигурации всех типов уведомлений")]
        public List<NotificationConfig> NotificationConfigs { get; private set; }

        /// <summary> Время показа уведомления на экране. </summary>
        [field: SerializeField, Tooltip("Время отображения (в секундах).")]
        public float DisplayTime { get; private set; }

        /// <summary> Время появления и исчезновения уведомления. </summary>
        [field: SerializeField, Tooltip("Время появления и исчезновения (в секундах).")]
        public float FadeTime { get; private set; }

        /// <summary> Возвращает горизонтальный отступ в зависимости от позиции. </summary>
        /// <param name="position"> Позиция уведомления. </param>
        /// <returns> Горизонтальный отступ. </returns>
        public float GetHorizontalPadding(NotificationPosition position) => position switch
        {
            NotificationPosition.TopLeft => PaddingLeft,
            NotificationPosition.BottomLeft => PaddingLeft,
            NotificationPosition.TopRight => PaddingRight,
            NotificationPosition.BottomRight => PaddingRight,
            _ => 0f
        };

        /// <summary> Возвращает вертикальный отступ в зависимости от позиции. </summary>
        /// <param name="position"> Позиция уведомления. </param>
        /// <returns> Вертикальный отступ. </returns>
        public float GetVerticalPadding(NotificationPosition position) => position switch
        {
            NotificationPosition.TopLeft => PaddingTop,
            NotificationPosition.TopRight => PaddingTop,
            NotificationPosition.BottomLeft => PaddingBottom,
            NotificationPosition.BottomRight => PaddingBottom,
            _ => 0f
        };
    }
}