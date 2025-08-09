using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Компонент для отображения 3D-индикатора над NPC с анимацией появления, исчезновения и вращения. </summary>
    public class NpcPurchaseIndicator : MonoBehaviour
    {
        [Tooltip("3D-модель индикатора, которую нужно анимировать.")] [SerializeField]
        private GameObject _model;

        [Header("Настройки анимации")]
        [Tooltip("Длительность анимации появления/исчезновения в секундах.")]
        [SerializeField]
        private float _fadeDuration = 0.6f;

        [Tooltip("Начальный масштаб при появлении.")] [SerializeField]
        private float _startScale = 0.8f;

        [Tooltip("Скорость вращения в градусах в секунду.")] [SerializeField]
        private float _rotationSpeed = 60f;

        /// <summary> Исходный масштаб модели. </summary>
        private Vector3 _defaultScale;

        /// <summary> Текущий tween, чтобы отменять при повторных вызовах. </summary>
        private Tween _currentTween;

        /// <summary> Флаг, что объект сейчас активен и крутится. </summary>
        private bool _isActive;

        private void Awake()
        {
            if (_model == null)
            {
                Debug.LogError($"Model is not assigned on {gameObject.name}");
                return;
            }

            _defaultScale = _model.transform.localScale;
            _model.transform.localScale = _defaultScale * _startScale;
            _model.SetActive(false);
        }

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