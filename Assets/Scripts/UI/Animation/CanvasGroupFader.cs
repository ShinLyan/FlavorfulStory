using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.UI.Animation
{
    /// <summary> Компонент для управления плавным появлением и исчезновением UI через CanvasGroup. </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        /// <summary> Компонент CanvasGroup для управления альфой и взаимодействием. </summary>
        private CanvasGroup _canvasGroup;

        /// <summary> Активная tween-анимация. </summary>
        private Tween _fadeTween;

        /// <summary> Длительность анимации показа по умолчанию. </summary>
        private const float DefaultFadeIn = 0.4f;

        /// <summary> Длительность анимации скрытия по умолчанию. </summary>
        private const float DefaultFadeOut = 0.2f;

        /// <summary> Easing-кривая по умолчанию. </summary>
        private const Ease DefaultEase = Ease.InOutSine;

        /// <summary> Кэширует CanvasGroup. </summary>
        private void Awake() => _canvasGroup = GetComponent<CanvasGroup>();

        /// <summary> Показывает элемент с настройками по умолчанию. </summary>
        public Tween Show() => FadeTo(1f, true, true, DefaultFadeIn, DefaultEase);

        /// <summary> Показывает элемент с кастомной длительностью и альфой. </summary>
        public Tween Show(float duration, float targetAlpha = 1f, Ease? ease = null)
            => FadeTo(Mathf.Clamp01(targetAlpha), true, true, duration, ease ?? DefaultEase);

        /// <summary> Скрывает элемент с настройками по умолчанию. </summary>
        public Tween Hide() => FadeTo(0f, false, false, DefaultFadeOut, DefaultEase);

        /// <summary> Скрывает элемент с кастомной длительностью. </summary>
        public Tween Hide(float duration, Ease? ease = null)
            => FadeTo(0f, false, false, duration, ease ?? DefaultEase);

        /// <summary> Выполняет анимацию изменения прозрачности до заданного значения. </summary>
        /// <param name="targetAlpha"> Целевое значение альфа-прозрачности. </param>
        /// <param name="interactable"> Должен ли элемент быть интерактивным после анимации. </param>
        /// <param name="blocksRaycasts"> Должен ли элемент блокировать лучи после анимации. </param>
        /// <param name="duration"> Продолжительность анимации. </param>
        /// <param name="ease"> Easing-кривая. </param>
        /// <returns> Tween-анимация изменения прозрачности. </returns>
        public Tween FadeTo(float targetAlpha, bool interactable, bool blocksRaycasts, float duration, Ease ease)
        {
            _fadeTween?.Kill();

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _fadeTween = _canvasGroup.DOFade(Mathf.Clamp01(targetAlpha), duration).SetEase(ease).OnComplete(() =>
            {
                _canvasGroup.interactable = interactable;
                _canvasGroup.blocksRaycasts = blocksRaycasts;
            });

            return _fadeTween;
        }

        /// <summary> Мгновенно применяет альфу и интерактивность без анимации. </summary>
        /// <param name="alpha"> Значение прозрачности. </param>
        /// <param name="interactable"> Должен ли элемент быть интерактивным. </param>
        public void SetState(float alpha, bool interactable)
        {
            _fadeTween?.Kill();
            _canvasGroup.alpha = Mathf.Clamp01(alpha);
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = interactable;
        }
    }
}