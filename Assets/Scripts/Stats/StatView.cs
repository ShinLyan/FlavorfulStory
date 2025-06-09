using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.Stats
{
    /// <summary> Отображение стата с прогресс-баром и подсказкой при наведении. </summary>
    public class StatView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Компонент ползунка, отображающий значения. </summary>
        [Header("UI References")]
        [SerializeField] private Slider _slider;

        /// <summary> Контейнер для текстовых значений. </summary>
        [SerializeField] private Transform _valueContainer;

        /// <summary> Текст текущего значения. </summary>
        [SerializeField] private TMP_Text _currentValueText;

        /// <summary> Текст максимального значения. </summary>
        [SerializeField] private TMP_Text _maxValueText;

        /// <summary> Показывать значения всегда, без необходимости наведения. </summary>
        [Header("Settings")]
        [SerializeField] private bool _alwaysShowValues;

        /// <summary> Максимальный коэффициент масштабирования полосы. </summary>
        private const float MaxWidthMultiplier = 2f;

        /// <summary> Текущее максимальное значение. </summary>
        private float _maxValue;

        /// <summary> RectTransform ползунка. </summary>
        private RectTransform _sliderRect;

        /// <summary> Базовая ширина полосы прогресса. </summary>
        private float _baseBarWidth;

        /// <summary> Инициализировать параметры ширины и RectTransform. </summary>
        private void Awake()
        {
            _sliderRect = _slider.GetComponent<RectTransform>();
            _baseBarWidth = _sliderRect.rect.width;
        }

        /// <summary> Инициализировать отображение значениями и масштабом. </summary>
        /// <param name="currentValue"> Начальное текущее значение. </param>
        /// <param name="maxValue"> Начальное максимальное значение. </param>
        public void Initialize(float currentValue, float maxValue)
        {
            UpdateMaxValue(maxValue);
            UpdateCurrentValue(currentValue);
            UpdateTextVisibility(_alwaysShowValues);
        }

        /// <summary> Обновить текущее значение и позицию ползунка. </summary>
        /// <param name="currentValue"> Новое значение. </param>
        public void UpdateCurrentValue(float currentValue)
        {
            _currentValueText.text = Format(currentValue);
            _slider.value = Mathf.Clamp01(currentValue / Mathf.Max(1f, _maxValue));
        }

        /// <summary> Преобразовать числовое значение в строку. </summary>
        /// <param name="value"> Значение. </param>
        /// <returns> Целочисленное строковое представление. </returns>
        private static string Format(float value) => ((int)value).ToString();

        /// <summary> Установить и отобразить новое максимальное значение. </summary>
        /// <param name="maxValue"> Новое максимальное значение. </param>
        public void UpdateMaxValue(float maxValue)
        {
            _maxValue = maxValue;
            _maxValueText.text = Format(maxValue);
            UpdateBarWidth();
        }

        /// <summary> Перерасчитать ширину полосы в зависимости от нового максимума. </summary>
        private void UpdateBarWidth()
        {
            float maxWidthLimit = _baseBarWidth * MaxWidthMultiplier;
            float widthRatio = Mathf.Clamp01(_maxValue / maxWidthLimit);
            float targetWidth = Mathf.Lerp(_baseBarWidth, maxWidthLimit, widthRatio);

            var size = _sliderRect.sizeDelta;
            size.x = targetWidth;
            _sliderRect.sizeDelta = size;
        }

        /// <summary> Отобразить значения при наведении курсора. </summary>
        /// <param name="eventData"> Данные события наведения. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_alwaysShowValues) UpdateTextVisibility(true);
        }

        /// <summary> Скрыть значения при выходе курсора. </summary>
        /// <param name="eventData"> Данные события выхода. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_alwaysShowValues) UpdateTextVisibility(false);
        }

        /// <summary> Показать или скрыть текстовые значения. </summary>
        /// <param name="visible"> true — показать; false — скрыть. </param>
        private void UpdateTextVisibility(bool visible) => _valueContainer.gameObject.SetActive(visible);
    }
}