using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory
{
    public enum PanelAnimationDirection { Top, Bottom, Left, Right, Center }

    public class AnimatedWindow : BaseWindow
    {
        [Header("Animation Settings")]
        [SerializeField] private RectTransform _container;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _animationEase = Ease.InOutBack;
        [SerializeField] private PanelAnimationDirection _direction = PanelAnimationDirection.Center;

        private Vector2 _offscreenPosition;
        private Vector2 _initialAnchoredPos;
        private bool _isAnimating;
        private bool _isFirstOpen = true;

        protected override async void OnOpened()
        {
            if (_container == null || _canvasGroup == null)
            {
                Debug.LogError($"[AnimatedWindow] Missing _container or _canvasGroup on {gameObject.name}");
                return;
            }

            if (_isFirstOpen)
            {
                _initialAnchoredPos = _container.anchoredPosition;
                SetOffscreenPosition();
                _isFirstOpen = false;
            }

            _container.anchoredPosition = _offscreenPosition;
            _container.localScale = _direction == PanelAnimationDirection.Center ? Vector3.one * 0.5f : Vector3.one;
            _canvasGroup.alpha = 0f;
            BlockRaycasts(true);
            _isAnimating = true;

            var sequence = DOTween.Sequence();
            sequence.Join(_canvasGroup.DOFade(1f, _animationDuration));
            sequence.Join(_container.DOAnchorPos(_initialAnchoredPos, _animationDuration).SetEase(_animationEase));
            if (_direction == PanelAnimationDirection.Center)
                sequence.Join(_container.DOScale(1f, _animationDuration).SetEase(_animationEase));

            await sequence.AsyncWaitForCompletion();
            _isAnimating = false;
            BlockRaycasts(false);
        }

        public override void Close()
        {
            if (!IsOpened || _isAnimating) return;
            CloseAsync().Forget();
        }

        private async UniTaskVoid CloseAsync()
        {
            _isAnimating = true;
            BlockRaycasts(true);

            var sequence = DOTween.Sequence();
            sequence.Join(_canvasGroup.DOFade(0f, _animationDuration));
            sequence.Join(_container.DOAnchorPos(_offscreenPosition, _animationDuration).SetEase(_animationEase));
            if (_direction == PanelAnimationDirection.Center)
                sequence.Join(_container.DOScale(0.5f, _animationDuration).SetEase(_animationEase));

            await sequence.AsyncWaitForCompletion();

            base.Close();
            _isAnimating = false;
        }

        protected override async void OnClosed()
        {
            if (_isAnimating || _container == null || _canvasGroup == null) return;

            _isAnimating = true;
            BlockRaycasts(true);

            var sequence = DOTween.Sequence();
            sequence.Join(_canvasGroup.DOFade(0f, _animationDuration));
            sequence.Join(_container.DOAnchorPos(_offscreenPosition, _animationDuration).SetEase(_animationEase));
            if (_direction == PanelAnimationDirection.Center)
                sequence.Join(_container.DOScale(0.5f, _animationDuration).SetEase(_animationEase));

            await sequence.AsyncWaitForCompletion();

            _isAnimating = false;
            BlockRaycasts(false);
        }

        private void SetOffscreenPosition()
        {
            var rect = _container.rect;
            var screenSize = ((RectTransform) _container.parent).rect.size;

            _offscreenPosition = _initialAnchoredPos;

            switch (_direction)
            {
                case PanelAnimationDirection.Top:
                    _offscreenPosition.y = screenSize.y + rect.height; break;
                case PanelAnimationDirection.Bottom:
                    _offscreenPosition.y = -screenSize.y - rect.height; break;
                case PanelAnimationDirection.Left:
                    _offscreenPosition.x = -screenSize.x - rect.width; break;
                case PanelAnimationDirection.Right:
                    _offscreenPosition.x = screenSize.x + rect.width; break;
                case PanelAnimationDirection.Center:
                    break;
            }
        }

        private void BlockRaycasts(bool block)
        {
            _canvasGroup.blocksRaycasts = !block;
            _canvasGroup.interactable = !block;
        }
    }
}