using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using FlavorfulStory.UI.Notifications;

namespace FlavorfulStory.Infrastructure.Configs.Notifications
    {
        /// <summary> Глобальные настройки системы уведомлений. </summary>
        [CreateAssetMenu(menuName = "FlavorfulStory/Notifications/SystemSettings")]
        public class NotificationSystemSettings : ScriptableObject
        {
            /// <summary> Время перемещения уведомлений при перестроении. </summary>
            public float MoveTime;
            /// <summary> Расстояние между уведомлениями в стеке. </summary>
            public float StackSpacing;
            /// <summary> Стандартная функция easing для анимаций. </summary>
            public Ease DefaultEasing;

            /// <summary> Отступ сверху экрана для уведомлений. </summary>
            [Header("Screen Padding")]
            public float PaddingTop;
            /// <summary> Отступ снизу экрана для уведомлений. </summary>
            public float PaddingBottom;
            /// <summary> Отступ слева экрана для уведомлений. </summary>
            public float PaddingLeft;
            /// <summary> Отступ справа экрана для уведомлений. </summary>
            public float PaddingRight;

            /// <summary> Список конфигураций всех типов уведомлений. </summary>
            public List<NotificationConfig> NotificationConfigs;

            /// <summary> Возвращает горизонтальный отступ в зависимости от позиции. </summary>
            /// <param name="position"> Позиция уведомления. </param>
            /// <returns> Горизонтальный отступ. </returns>
            public float GetHorizontalPadding(NotificationPosition position) => position switch
            {
                NotificationPosition.TopLeft     => PaddingLeft,
                NotificationPosition.BottomLeft  => PaddingLeft,
                NotificationPosition.TopRight    => PaddingRight,
                NotificationPosition.BottomRight => PaddingRight,
                _ => 0f
            };

            /// <summary> Возвращает вертикальный отступ в зависимости от позиции. </summary>
            /// <param name="pos"> Позиция уведомления. </param>
            /// <returns> Вертикальный отступ. </returns>
            public float GetVerticalPadding(NotificationPosition pos) => pos switch
            {
                NotificationPosition.TopLeft     => PaddingTop,
                NotificationPosition.TopRight    => PaddingTop,
                NotificationPosition.BottomLeft  => PaddingBottom,
                NotificationPosition.BottomRight => PaddingBottom,
                _ => 0f
            };
        }
    }