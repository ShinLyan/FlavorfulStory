using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory
{
    /// <summary> World-space UI для процессора. LayoutRoot с HorizontalLayoutGroup. </summary>
    public class ResourceProcessorUIView : MonoBehaviour
    {
        [Header("Canvas")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private bool _faceCamera = true;

        [Header("Anchoring")]
        [SerializeField] private Vector3 _offset = new(0f, 1.6f, 0f);
        private Transform _target;
        private Camera _cam;

        [Header("Layout")]
        [SerializeField] private RectTransform _layoutRoot;     // на нём HorizontalLayoutGroup
        [SerializeField] private GameObject _inputGroup;        // контейнер инпута
        [SerializeField] private GameObject _arrowGO;           // стрелка
        [SerializeField] private GameObject _outputGroup;       // контейнер аутпута

        [Header("Input UI")]
        [SerializeField] private Image _inputIcon;
        [SerializeField] private TextMeshProUGUI _inputText;

        [Header("Output UI")]
        [SerializeField] private Image _outputIcon;
        [SerializeField] private TextMeshProUGUI _outputText;
        [SerializeField] private Image _outputFill; // type = Filled, Vertical, Bottom

        [Header("Tweens")]
        [SerializeField] private float _fadeDuration = 0.15f;

        private Tween _fillTween;

        private void Awake()
        {
            if (_canvas && _canvas.renderMode != RenderMode.WorldSpace)
                _canvas.renderMode = RenderMode.WorldSpace;

            _cam = Camera.main;

            // Старт: инпут виден, остальное скрыто
            SetProcessWidgetsVisible(false);
            SetFill01(0f, true);
        }

        private void LateUpdate()
        {
            if (_target) transform.position = _target.position + _offset;

            if (_faceCamera && _cam)
            {
                var fwd = transform.position - _cam.transform.position;
                fwd.y = 0f;
                if (fwd.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(fwd);
            }
        }

        // ---------- Публичное API ----------

        public void AttachTo(Transform target) => _target = target;
        public void SetOffset(Vector3 offset) => _offset = offset;

        /// <summary> Показ/скрытие всей вьюхи при входе/выходе из триггера. </summary>
        public void SetVisible(bool visible)
        {
            if (_canvasGroup)
            {
                _canvasGroup.DOKill();
                _canvasGroup.alpha = visible ? 1f : 0f;
                _canvasGroup.interactable = visible;
                _canvasGroup.blocksRaycasts = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        /// <summary> Инициализация спрайтов и статичных подписей. </summary>
        public void Setup(Sprite inputSprite, Sprite outputSprite, int outputNumber)
        {
            if (_inputIcon)  _inputIcon.sprite  = inputSprite;
            if (_outputIcon) _outputIcon.sprite = outputSprite;
            if (_outputText) _outputText.text   = outputNumber.ToString();

            // Инпут всегда активен, остальное выключим до старта процесса
            if (_inputGroup) _inputGroup.SetActive(true);
            SetProcessWidgetsVisible(false);
            SetFill01(0f, true);
        }

        /// <summary> Обновление счётчика инпута. НЕ трогает стрелку/аутпут. </summary>
        public void SetQueue(int itemsLeft, int capacity)
        {
            if (_inputText) _inputText.text = $"{itemsLeft}/1";
        }

        /// <summary> Включает/выключает элементы процесса (стрелка+аутпут). </summary>
        public void SetProcessingState(bool isProcessing)
        {
            SetProcessWidgetsVisible(isProcessing);

            if (!isProcessing)
            {
                _fillTween?.Kill();
                SetFill01(0f, true);
            }
        }

        /// <summary> Запуск анимации заливки одного цикла. </summary>
        public void StartCycle(float duration)
        {
            _fillTween?.Kill();
            SetFill01(0f, true);

            if (!_outputFill) return;

            if (duration <= 0f)
            {
                _outputFill.fillAmount = 1f;
                return;
            }

            _fillTween = _outputFill.DOFillAmount(1f, duration)
                                    .SetEase(Ease.Linear)
                                    .SetUpdate(true);
        }

        /// <summary> Сброс заливки по завершению цикла. </summary>
        public void CompleteCycle() => SetFill01(0f, true);

        // ---------- helpers ----------

        private void SetProcessWidgetsVisible(bool visible)
        {
            if (_arrowGO)     _arrowGO.SetActive(visible);
            if (_outputGroup) _outputGroup.SetActive(visible);
            if (_inputGroup)  _inputGroup.SetActive(true); // инпут ВСЕГДА виден

            // HorizontalLayoutGroup сам выстроит:
            // - idle: активен только InputGroup → по центру
            // - processing: все три активны → input-left, arrow-center, output-right
        }

        private void SetFill01(float t, bool instant)
        {
            if (!_outputFill) return;
            t = Mathf.Clamp01(t);
            if (instant) _outputFill.fillAmount = t;
            else _outputFill.DOFillAmount(t, _fadeDuration).SetUpdate(true);
        }
    }
}