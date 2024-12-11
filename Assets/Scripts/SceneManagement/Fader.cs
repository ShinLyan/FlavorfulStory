using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Coroutine _currentActiveFade;

        public const float FadeOutTime = 0.5f;
        public const float FadeInTime = 2f;
        public const float FadeWaitTime = 0.5f;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate() => _canvasGroup.alpha = 1f;

        public void FadeInImmediate() => _canvasGroup.alpha = 0f;

        public Coroutine FadeOut(float time) => Fade(1f, time);

        public Coroutine FadeIn(float time) => Fade(0f, time);

        private Coroutine Fade(float target, float time)
        {
            if (_currentActiveFade != null) StopCoroutine(_currentActiveFade);

            _currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return _currentActiveFade;
        }

        private System.Collections.IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}