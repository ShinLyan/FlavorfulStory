using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace FlavorfulStory
{
    public class PickupNotification : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text message;
        [SerializeField] private CanvasGroup canvasGroup;

        private RectTransform _rectTransform;
        private Tween _fadeTween;
        private float _startX;

        public float Height => _rectTransform.rect.height;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _startX = _rectTransform.anchoredPosition.x;
        }

        public void Initialize(Sprite itemSprite, string text)
        {
            icon.sprite = itemSprite;
            message.text = text;
            canvasGroup.alpha = 0f;

            float offscreenX = _startX + Screen.width;
            _rectTransform.anchoredPosition = new Vector2(offscreenX, _rectTransform.anchoredPosition.y);
        }

        public void Show(float fadeDuration)
        {
            _fadeTween?.Kill();

            DOTween.Sequence()
                .Join(canvasGroup.DOFade(1f, fadeDuration))
                .Join(_rectTransform.DOAnchorPosX(_startX, fadeDuration).SetEase(Ease.OutCubic));
        }

        public void MoveUp(float distance, float duration)
        {
            _rectTransform
                .DOAnchorPosY(_rectTransform.anchoredPosition.y + distance, duration)
                .SetEase(Ease.OutCubic);
        }

        public void FadeAndDestroy(float duration)
        {
            _fadeTween?.Kill();

            float offscreenX = _startX + Screen.width;

            DOTween.Sequence()
                .Join(canvasGroup.DOFade(0f, duration))
                .Join(_rectTransform.DOAnchorPosX(offscreenX, duration).SetEase(Ease.InCubic))
                .OnComplete(() => Destroy(gameObject));
        }
    }
}