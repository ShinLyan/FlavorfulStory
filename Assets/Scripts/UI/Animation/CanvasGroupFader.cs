using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.UI.Animation
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Tween _fadeTween;

        private const float DefaultFadeIn = 0.4f;
        private const float DefaultFadeOut = 0.2f;
        private const Ease DefaultEase = Ease.InOutSine;

        private void Awake() => _canvasGroup = GetComponent<CanvasGroup>();
        
        public Tween Show() => FadeTo(1f, true, true, DefaultFadeIn, DefaultEase);
        
        public Tween Show(float duration, float targetAlpha = 1f, Ease? ease = null)
            => FadeTo(Mathf.Clamp01(targetAlpha), true, true, duration, ease ?? DefaultEase);
        
        public Tween Hide() => FadeTo(0f, false, false, DefaultFadeOut, DefaultEase);

        public Tween Hide(float duration, Ease? ease = null)
            => FadeTo(0f, false, false, duration, ease ?? DefaultEase);
        
        public Tween FadeTo(float targetAlpha, bool interactable, bool blocksRaycasts, float duration, Ease ease)
        {
            _fadeTween?.Kill();
            
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _fadeTween = _canvasGroup
                .DOFade(Mathf.Clamp01(targetAlpha), duration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    _canvasGroup.interactable = interactable;
                    _canvasGroup.blocksRaycasts = blocksRaycasts;
                });

            return _fadeTween;
        }
        
        public void SetState(float alpha, bool interactable, bool blocksRaycasts)
        {
            _fadeTween?.Kill();
            _canvasGroup.alpha = Mathf.Clamp01(alpha);
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = blocksRaycasts;
        }
    }
}