using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Управляет постепенным появлением и исчезновением CanvasGroup. </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {
        /// <summary> Компонент CanvasGroup для управления прозрачностью. </summary>
        private CanvasGroup _canvasGroup;

        /// <summary> Активная корутина анимации затемнения. </summary>
        private Coroutine _currentActiveFade;

        /// <summary> Время для полного исчезновения. </summary>
        public const float FadeOutTime = 0.5f;

        /// <summary> Время для полного появления. </summary>
        public const float FadeInTime = 2f;

        /// <summary> Время ожидания перед началом анимации. </summary>
        public const float FadeWaitTime = 0.5f;

        /// <summary> Инициализирует компонент CanvasGroup. </summary>
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary> Немедленно устанавливает CanvasGroup на полностью прозрачное состояние. </summary>
        public void FadeOutImmediate() => _canvasGroup.alpha = 1f;

        /// <summary> Немедленно устанавливает CanvasGroup на полностью видимое состояние. </summary>
        public void FadeInImmediate() => _canvasGroup.alpha = 0f;

        /// <summary> Постепенно затемняет CanvasGroup. </summary>
        /// <param name="time"> Время анимации. </param>
        /// <returns> Корутин для управления затемнением. </returns>
        public Coroutine FadeOut(float time) => Fade(1f, time);

        /// <summary> Постепенно делает CanvasGroup видимым. </summary>
        /// <param name="time"> Время анимации. </param>
        /// <returns> Корутин для управления появлением. </returns>
        public Coroutine FadeIn(float time) => Fade(0f, time);

        /// <summary> Запускает анимацию затемнения или появления. </summary>
        /// <param name="target"> Целевое значение прозрачности (0 для появления, 1 для затемнения). </param>
        /// <param name="time"> Время анимации. </param>
        /// <returns> Корутин для управления процессом анимации. </returns>
        private Coroutine Fade(float target, float time)
        {
            if (_currentActiveFade != null) StopCoroutine(_currentActiveFade);

            _currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return _currentActiveFade;
        }

        /// <summary> Процесс анимации прозрачности CanvasGroup. </summary>
        /// <param name="target"> Целевое значение прозрачности. </param>
        /// <param name="time"> Время анимации. </param>
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