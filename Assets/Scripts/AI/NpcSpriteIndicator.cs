using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Компонент для отображения спрайта-индикатора над NPC с анимацией появления и исчезновения. </summary>
    public class NpcSpriteIndicator : MonoBehaviour
    {
        [Tooltip("Canvas с UI Image, который нужно анимировать.")]
        [SerializeField]
        private GameObject _sprite;

        [Header("Настройки анимации")]
        [Tooltip("Длительность анимации появления/исчезновения в секундах.")]
        [SerializeField]
        private float _fadeDuration = 0.6f;

        [Tooltip("Вертикальное смещение при анимации (эффект подпрыгивания/ухода).")] [SerializeField]
        private float _popHeight = 0.2f;

        [Tooltip("Тип сглаживания (Ease) для анимации перемещения.")] [SerializeField]
        private Ease _easeType = Ease.OutBack;

        [Tooltip("Начальный масштаб при появлении.")] [SerializeField]
        private float _startScale = 0.8f;

        /// <summary> CanvasGroup для управления прозрачностью спрайта. </summary>
        private CanvasGroup _canvasGroup;

        /// <summary> Базовая локальная позиция спрайта (без смещений). </summary>
        private Vector3 _defaultLocalPos;

        /// <summary> Текущая анимация (Tween), чтобы можно было её отменить при новой. </summary>
        private Tween _currentTween;

        private void Awake()
        {
            if (_sprite == null)
            {
                Debug.LogError($"Sprite indicator needs to be assigned to {gameObject.name}");
                return;
            }

            _defaultLocalPos = _sprite.transform.localPosition;

            _canvasGroup = _sprite.GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = _sprite.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0;
            _sprite.transform.localScale = Vector3.one * _startScale;
            _sprite.SetActive(false);
        }

        /// <summary> Показать спрайт с анимацией. </summary>
        public async UniTask ShowSprite()
        {
            _currentTween?.Kill();
            _sprite.SetActive(true);
            PrepareForShowAnimation();
            _currentTween = CreateShowAnimation();
            await _currentTween.AsyncWaitForCompletion();
        }

        /// <summary> Скрыть спрайт с анимацией. </summary>
        public async UniTask HideSprite()
        {
            _currentTween?.Kill();
            _currentTween = CreateHideAnimation();
            await _currentTween.AsyncWaitForCompletion();
            _sprite.SetActive(false);
        }

        /// <summary> Подготовка объекта перед анимацией появления. </summary>
        private void PrepareForShowAnimation()
        {
            _sprite.transform.localPosition = _defaultLocalPos - Vector3.up * _popHeight;
            _sprite.transform.localScale = Vector3.one * _startScale;
            _canvasGroup.alpha = 0;
        }

        /// <summary> Создаёт анимацию появления спрайта. </summary>
        private Tween CreateShowAnimation()
        {
            return DOTween.Sequence()
                .Join(_canvasGroup.DOFade(1f, _fadeDuration))
                .Join(_sprite.transform.DOLocalMoveY(_defaultLocalPos.y, _fadeDuration).SetEase(_easeType))
                .Join(_sprite.transform.DOScale(1f, _fadeDuration).SetEase(_easeType));
        }

        /// <summary> Создаёт анимацию исчезновения спрайта. </summary>
        private Tween CreateHideAnimation()
        {
            return DOTween.Sequence()
                .Join(_canvasGroup.DOFade(0f, _fadeDuration))
                .Join(_sprite.transform.DOLocalMoveY(_defaultLocalPos.y + _popHeight, _fadeDuration / 1.5f))
                .Join(_sprite.transform.DOScale(_startScale, _fadeDuration / 1.5f));
        }
    }
}