using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace FlavorfulStory.UI.Windows
{
    /// <summary> Направление анимации появления окна. </summary>
    public enum PanelAnimationDirection { Top, Bottom, Left, Right, Center }

    /// <summary> Окно с анимацией появления и исчезновения (скрытие за экран / масштаб / fade). </summary>
    public class AnimatedWindow : BaseWindow
    {
        /// <summary> Контейнер UI-элемента, который будет анимироваться. </summary>
        [Header("Animation Settings")]
        [SerializeField] private RectTransform _container;
        /// <summary> CanvasGroup для управления прозрачностью и блокировкой ввода. </summary>
        [SerializeField] private CanvasGroup _canvasGroup;
        /// <summary> Длительность анимации появления/исчезновения. </summary>
        [SerializeField] private float _animationDuration;
        /// <summary> Кривая анимации (easing). </summary>
        [SerializeField] private Ease _animationEase;
        /// <summary> Направление появления окна. </summary>
        [SerializeField] private PanelAnimationDirection _direction;

        /// <summary> Расчитанная позиция за экраном, откуда появляется окно. </summary>
        private Vector2 _offscreenPosition;
        /// <summary> Исходная позиция контейнера в пространстве родителя. </summary>
        private Vector2 _initialAnchoredPos;
        /// <summary> Флаг, предотвращающий повторные анимации. </summary>
        private bool _isAnimating;
        /// <summary> Открывается ли окно впервые. </summary>
        private bool _isFirstOpen = true;
        
        /// <summary> Обработчик события открытия — запускает анимацию появления. </summary>
        protected override void OnOpened() => AnimateOpenAsync().Forget();

        /// <summary> Асинхронная анимация появления окна. </summary>
        private async UniTaskVoid AnimateOpenAsync()
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
        
        /// <summary> Переопределение закрытия — запускает анимацию закрытия. </summary>
        public override void Close()
        {
            if (!IsOpened || _isAnimating) return;
            CloseAsync().Forget();
        }

        /// <summary> Асинхронная анимация скрытия и закрытие окна. </summary>
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
        
        /// <summary> Вызывается при закрытии окна через базовый метод. </summary>
        protected override void OnClosed() => AnimateCloseOnClosedAsync().Forget();

        /// <summary> Анимация закрытия, если окно закрыто через WindowService. </summary>
        private async UniTaskVoid AnimateCloseOnClosedAsync()
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
        
        /// <summary> Рассчитывает позицию окна за экраном в зависимости от направления. </summary>
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

        /// <summary> Включает/выключает блокировку ввода и интерактивность. </summary>
        private void BlockRaycasts(bool block)
        {
            _canvasGroup.blocksRaycasts = !block;
            _canvasGroup.interactable = !block;
        }
    }
}