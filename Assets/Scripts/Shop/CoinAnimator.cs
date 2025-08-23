using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.Shop
{
    /// <summary> Управляет анимацией монетки с использованием DOTween и UniTask. </summary>
    public class CoinAnimator : MonoBehaviour
    {
        /// <summary> Модель монетки. </summary>
        [Header("Coin model")] [Tooltip("Coin model."), SerializeField]
        private GameObject _coinModel;

        /// <summary> Амплитуда подпрыгивания. </summary>
        [Header("Bounce settings")] [Tooltip("Амплитуда подпрыгивания монетки."), SerializeField]
        private float _bounceAmplitude = 0.1f;

        /// <summary> Длительность прыжка. </summary>
        [Tooltip("Длительность одного прыжка (вверх-вниз)."), SerializeField]
        private float _bounceDuration = 0.5f;

        /// <summary> Скорость вращения. </summary>
        [Header("Rotation settings")]
        [Tooltip("Скорость вращения (градусов в секунду)."), SerializeField]
        private float _rotationSpeed = 180f;

        /// <summary> Позиция по умолчанию. </summary>
        private Vector3 _defaultPosition;

        /// <summary> Текущая анимация перемещения. </summary>
        private Tween _currentTween;

        /// <summary> Анимация вращения. </summary>
        private Tween _rotationTween;

        /// <summary> Анимация подпрыгивания. </summary>
        private Tween _bounceTween;

        /// <summary> Инициализация компонента. </summary>
        private void Awake()
        {
            _defaultPosition = _coinModel.transform.localPosition;
            _coinModel.SetActive(false);
        }

        /// <summary> Останавливает анимации при уничтожении. </summary>
        private void OnDestroy() => KillAnimations();

        /// <summary> Переключает видимость монетки с анимацией. </summary>
        /// <param name="isVisible"> Показывать монетку. </param>
        public void ToggleCoin(bool isVisible) => ToggleCoinAsync(isVisible).Forget();

        /// <summary> Асинхронно переключает видимость монетки. </summary>
        /// <param name="isVisible"> Показывать монетку. </param>
        private async UniTaskVoid ToggleCoinAsync(bool isVisible)
        {
            KillAnimations();

            if (isVisible)
            {
                _coinModel.SetActive(true);
                _coinModel.transform.localPosition = _defaultPosition;
                
                float rotationDuration = 360f / _rotationSpeed;
                _rotationTween = _coinModel.transform
                    .DOLocalRotate(new Vector3(0f, 360f, 0f), rotationDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart)
                    .SetRelative();
                
                _bounceTween = _coinModel.transform
                    .DOLocalMoveY(_defaultPosition.y + _bounceAmplitude, _bounceDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);

                await UniTask.Yield();
            }
            else
            {
                _currentTween = _coinModel.transform
                    .DOLocalMoveY(_defaultPosition.y, 0.2f)
                    .SetEase(Ease.InOutSine);

                _coinModel.SetActive(false);
            }
        }

        /// <summary> Останавливает все активные анимации. </summary>
        private void KillAnimations()
        {
            _currentTween?.Kill(true);
            _rotationTween?.Kill(true);
            _bounceTween?.Kill(true);

            _currentTween = null;
            _rotationTween = null;
            _bounceTween = null;
        }
    }
}