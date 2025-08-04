using System;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> Находит контейнер (якорь) для отображения уведомлений в указанной позиции экрана. </summary>
    public class NotificationAnchorLocator : MonoBehaviour
    {
        /// <summary> Якорь в верхнем левом углу. </summary>
        [SerializeField] private RectTransform _topLeft;

        /// <summary> Якорь в верхнем правом углу. </summary>
        [SerializeField] private RectTransform _topRight;

        /// <summary> Якорь в нижнем левом углу. </summary>
        [SerializeField] private RectTransform _bottomLeft;

        /// <summary> Якорь в нижнем правом углу. </summary>
        [SerializeField] private RectTransform _bottomRight;

        /// <summary> Получить контейнер для уведомления в зависимости от позиции. </summary>
        /// <param name="position"> Желаемая позиция уведомления на экране. </param>
        /// <returns> RectTransform-якорь, соответствующий позиции. </returns>
        /// <exception cref="Exception"> Выбрасывается, если позиция не распознана. </exception>
        public RectTransform GetContainer(NotificationPosition position) => position switch
        {
            NotificationPosition.TopLeft => _topLeft,
            NotificationPosition.TopRight => _topRight,
            NotificationPosition.BottomLeft => _bottomLeft,
            NotificationPosition.BottomRight => _bottomRight,
            _ => throw new Exception("Invalid notification position")
        };
    }
}