using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Компонент для отображения 3D-индикатора над NPC. </summary>
    public class NpcPurchaseIndicator : MonoBehaviour
    {
        /// <summary> 3D-модель индикатора. </summary>
        [Tooltip("3D-модель индикатора, которую нужно анимировать.")] [SerializeField]
        private GameObject _model;

        /// <summary> Длительность анимации. </summary>
        [Header("Настройки анимации")]
        [Tooltip("Длительность анимации появления/исчезновения в секундах.")]
        [SerializeField]
        private float _fadeDuration = 0.6f;

        /// <summary> Начальный масштаб. </summary>
        [Tooltip("Начальный масштаб при появлении.")] [SerializeField]
        private float _startScale = 0.8f;

        /// <summary> Скорость вращения. </summary>
        [Tooltip("Скорость вращения в градусах в секунду.")] [SerializeField]
        private float _rotationSpeed = 60f;

        /// <summary> Исходный масштаб модели. </summary>
        private Vector3 _defaultScale;

        /// <summary> Текущая анимация. </summary>
        private Tween _currentTween;

        /// <summary> Флаг активности индикатора. </summary>
        private bool _isActive;

        /// <summary> Инициализация компонента. </summary>
        private void Awake()
        {
            _defaultScale = _model.transform.localScale;
            _model.transform.localScale = _defaultScale * _startScale;
            _model.SetActive(false);
        }

        /// <summary> Обновление вращения. </summary>
        private void Update()
        {
            if (_isActive && _model) _model.transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);
        }

        /// <summary> Показать модель с анимацией появления. </summary>
        public async UniTask ShowModel()
        {
            _currentTween?.Kill();
            _model.SetActive(true);
            _model.transform.localScale = _defaultScale * _startScale;

            _currentTween = _model.transform.DOScale(_defaultScale, _fadeDuration).SetEase(Ease.OutBack);

            _isActive = true;
            await _currentTween.AsyncWaitForCompletion();
        }

        /// <summary> Скрыть модель с анимацией исчезновения. </summary>
        public async UniTask HideModel()
        {
            _currentTween?.Kill();

            _currentTween = _model.transform.DOScale(_defaultScale * _startScale, _fadeDuration);

            _isActive = false;
            await _currentTween.AsyncWaitForCompletion();
            _model.SetActive(false);
        }
    }
}