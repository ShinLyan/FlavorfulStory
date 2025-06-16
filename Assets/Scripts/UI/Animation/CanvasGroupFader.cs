using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.UI.Animation
{
    /// <summary> Компонент для управления плавным появлением и исчезновением UI через CanvasGroup. </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        /// <summary> Компонент CanvasGroup для управления прозрачностью и интерактивностью. </summary>
        private CanvasGroup _canvasGroup;

        /// <summary> Активная tween-анимация изменения прозрачности. </summary>
        private Tween _fadeTween;

        /// <summary> Длительность появления. </summary>
        private const float FadeInDuration = 0.4f;

        /// <summary> Длительность исчезновения. </summary>
        private const float FadeOutDuration = 0.3f;

        /// <summary> Инициализация компонента. </summary>
        private void Awake() => _canvasGroup = GetComponent<CanvasGroup>();

        /// <summary> Плавно показывает элемент. </summary>
        /// <returns> Tween-анимация, управляющая появлением. </returns>
        public Tween Show() => FadeTo(1f, true, FadeInDuration);

        /// <summary> Плавно скрывает элемент. </summary>
        /// <returns> Tween-анимация, управляющая исчезновением. </returns>
        public Tween Hide() => FadeTo(0f, false, FadeOutDuration);

        /// <summary> Мгновенно показывает элемент без анимации. </summary>
        public void ShowImmediate() => SetAlpha(1f, true);

        /// <summary> Мгновенно скрывает элемент без анимации. </summary>
        public void HideImmediate() => SetAlpha(0f, false);

        /// <summary> Выполняет анимацию изменения прозрачности до заданного значения. </summary>
        /// <param name="targetAlpha"> Целевое значение альфа-прозрачности. </param>
        /// <param name="interactable"> Должен ли элемент быть интерактивным после анимации. </param>
        /// <param name="duration"> Продолжительность анимации. </param>
        /// <returns> Tween-анимация изменения прозрачности. </returns>
        private Tween FadeTo(float targetAlpha, bool interactable, float duration)
        {
            _fadeTween?.Kill();

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            const Ease fadeEase = Ease.InOutSine;
            _fadeTween = _canvasGroup.DOFade(targetAlpha, duration).SetEase(fadeEase).OnComplete(() =>
            {
                _canvasGroup.interactable = interactable;
                _canvasGroup.blocksRaycasts = interactable;
            });

            return _fadeTween;
        }

        /// <summary> Устанавливает прозрачность и интерактивность элемента без анимации. </summary>
        /// <param name="alpha"> Значение прозрачности. </param>
        /// <param name="interactable"> Должен ли элемент быть интерактивным. </param>
        private void SetAlpha(float alpha, bool interactable)
        {
            _fadeTween?.Kill();
            _canvasGroup.alpha = alpha;
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = interactable;
        }
    }
}