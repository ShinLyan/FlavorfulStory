using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlavorfulStory
{
    /// <summary> UI-элемент уведомления о подобранном предмете. </summary>
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class BaseNotificationView : MonoBehaviour
    {
        public abstract void Initialize(INotificationData data);
        public abstract float Height { get; }
        public RectTransform RectTransform { get; private set; }
        public NotificationPosition Position { get; private set; }
        public float StartXPosition { get; private set; }

        [SerializeField] private RectTransform _backgroundRect;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            StartXPosition = RectTransform.anchoredPosition.x;
        }

        public void SetupPosition(NotificationPosition position, Vector2 padding)
        {
            Position = position;

            var anchor = position switch
            {
                NotificationPosition.TopLeft     => new Vector2(0f, 1f),
                NotificationPosition.TopRight    => new Vector2(1f, 1f),
                NotificationPosition.BottomLeft  => new Vector2(0f, 0f),
                NotificationPosition.BottomRight => new Vector2(1f, 0f),
                _ => Vector2.zero
            };

            ApplyToRect(RectTransform, anchor);
            ApplyToRect(_backgroundRect, anchor);

            // Применяем X-отступ от края экрана
            float xOffset = anchor.x == 0f ? padding.x : -padding.x;
            RectTransform.anchoredPosition = new Vector2(xOffset, 0f);

            StartXPosition = RectTransform.anchoredPosition.x;
        }

        private void ApplyToRect(RectTransform rect, Vector2 anchor)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = anchor;
        }

        public void Show(float fadeDuration)
        {
            _canvasGroup.DOFade(1f, fadeDuration);
            RectTransform.DOAnchorPosX(StartXPosition, fadeDuration).SetEase(Ease.OutCubic);
        }

        public void HideAndDestroy(float fadeDuration)
        {
            DOTween.Sequence()
                .Join(_canvasGroup.DOFade(0f, fadeDuration))
                .Join(RectTransform.DOAnchorPosX(-StartXPosition, fadeDuration))
                .OnComplete(() => Destroy(gameObject));
        }

        public void MoveTo(Vector2 target, float duration)
        {
            RectTransform.DOAnchorPos(target, duration).SetEase(Ease.OutCubic);
        }
    }
}