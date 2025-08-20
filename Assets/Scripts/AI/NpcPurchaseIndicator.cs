using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Управляет 3D-индикатором покупки над NPC. </summary>
    public class NpcPurchaseIndicator : MonoBehaviour
    {
        /// <summary> 3D-модель индикатора. </summary>
        [Tooltip("3D-модель индикатора, которую нужно анимировать."), SerializeField]
        private GameObject _model;

        /// <summary> Длительность анимации. </summary>
        [Header("Настройки анимации")]
        [Tooltip("Длительность анимации появления/исчезновения в секундах."), SerializeField]
        private float _fadeDuration = 0.6f;

        /// <summary> Начальный масштаб. </summary>
        [Tooltip("Начальный масштаб при появлении."), SerializeField]
        private float _startScale = 0.8f;

        /// <summary> Скорость вращения. </summary>
        [Tooltip("Скорость вращения (градусов в секунду)."), SerializeField]
        private float _rotationSpeed = 60f;

        /// <summary> Исходный масштаб модели. </summary>
        private Vector3 _defaultScale;

        /// <summary> Текущая анимация масштаба. </summary>
        private Tween _currentTween;

        /// <summary> Текущая анимация вращения. </summary>
        private Tween _rotationTween;

        /// <summary> Инициализация компонента. </summary>
        private void Awake()
        {
            _defaultScale = _model.transform.localScale;
            _model.transform.localScale = _defaultScale * _startScale;
            _model.SetActive(false);
        }

        /// <summary> Останавливает анимации при уничтожении. </summary>
        private void OnDestroy() => KillAnimations();

        /// <summary> Показывает индикатор с анимацией. </summary>
        /// <returns> Задача анимации. </returns>
        public async UniTask ShowModel()
        {
            KillAnimations();
            _model.SetActive(true);
            _model.transform.localScale = _defaultScale * _startScale;

            _currentTween = _model.transform
                .DOScale(_defaultScale, _fadeDuration)
                .SetEase(Ease.OutBack);

            float rotationDuration = 360f / _rotationSpeed;
            _rotationTween = _model.transform
                .DOLocalRotate(new Vector3(0f, 360f, 0f), rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetRelative();

            await _currentTween.AsyncWaitForCompletion();
        }

        /// <summary> Скрывает индикатор с анимацией. </summary>
        /// <returns> Задача анимации. </returns>
        public async UniTask HideModel()
        {
            KillAnimations();
            _currentTween = _model.transform
                .DOScale(_defaultScale * _startScale, _fadeDuration);

            await _currentTween.AsyncWaitForCompletion();
            _model.SetActive(false);
        }

        /// <summary> Останавливает все активные анимации. </summary>
        private void KillAnimations()
        {
            _currentTween?.Kill(true);
            _rotationTween?.Kill(true);
            _currentTween = null;
            _rotationTween = null;
        }
    }
}