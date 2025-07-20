using UnityEngine;

namespace FlavorfulStory
{
    public enum NotificationPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }
    
    public partial class BaseNotificationView : MonoBehaviour
    {
        [SerializeField] private RectTransform _backgroundRect;

        /// <summary> Применить Anchor и Pivot для уведомления и его контейнера в зависимости от позиции. </summary>
        public void ApplyAnchorAndPivot(NotificationPosition position)
        {
            Vector2 anchorPivot = position switch
            {
                NotificationPosition.TopLeft     => new Vector2(0f, 1f),
                NotificationPosition.TopRight    => new Vector2(1f, 1f),
                NotificationPosition.BottomLeft  => new Vector2(0f, 0f),
                NotificationPosition.BottomRight => new Vector2(1f, 0f),
                _ => Vector2.zero
            };

            ApplyToRect(RectTransform, anchorPivot);
            ApplyToRect(_backgroundRect, anchorPivot);

            RectTransform.anchoredPosition = Vector2.zero;
            StartXPosition = RectTransform.anchoredPosition.x;
        }

        private static void ApplyToRect(RectTransform rect, Vector2 anchorPivot)
        {
            rect.anchorMin = anchorPivot;
            rect.anchorMax = anchorPivot;
            rect.pivot = anchorPivot;
        }
    }
}