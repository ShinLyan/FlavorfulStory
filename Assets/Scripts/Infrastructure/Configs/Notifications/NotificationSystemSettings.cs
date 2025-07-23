using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using FlavorfulStory.UI.Notifications;

namespace FlavorfulStory.Infrastructure.Configs.Notifications
    {
        [CreateAssetMenu(menuName = "FlavorfulStory/Notifications/SystemSettings")]
        public class NotificationSystemSettings : ScriptableObject
        {
            public float MoveTime;
            public float StackSpacing;
            public Ease DefaultEasing;

            [Header("Screen Padding")]
            public float PaddingTop;
            public float PaddingBottom;
            public float PaddingLeft;
            public float PaddingRight;

            public List<NotificationConfig> NotificationConfigs;

            public float GetHorizontalPadding(NotificationPosition pos) => pos switch
            {
                NotificationPosition.TopLeft     => PaddingLeft,
                NotificationPosition.BottomLeft  => PaddingLeft,
                NotificationPosition.TopRight    => PaddingRight,
                NotificationPosition.BottomRight => PaddingRight,
                _ => 0f
            };

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