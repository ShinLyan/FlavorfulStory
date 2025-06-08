using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.Stats
{
    /// <summary> Отображение атрибута с визуальным прогресс-баром и подсказкой при наведении. </summary>
    public class HoverAttributeView : BaseAttributeView, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Контейнер полосы атрибута. </summary>
        [Header("Inspector refs")]
        [SerializeField] private RectTransform _contentRect;

        /// <summary> Изображение заливки полосы. </summary>
        [SerializeField] private Image _fillImage;

        /// <summary> Родитель текстовых элементов значений. </summary>
        [SerializeField] private Transform _textParent;

        /// <summary> Текст текущего значения. </summary>
        [SerializeField] private TMP_Text _currentValueText;

        /// <summary> Текст максимального значения. </summary>
        [SerializeField] private TMP_Text _maxValueText;

        /// <summary> Показывать значения всегда, без наведения. </summary>
        [Header("Settings")]
        [SerializeField] private bool _alwaysShowValues;

        /// <summary> Базовая ширина полосы. </summary>
        [SerializeField] private float _baseBarWidth = 200f;

        /// <summary> Максимальный коэффициент масштабирования полосы. </summary>
        [SerializeField] private float _maxScaleMultiplier = 2f;

        /// <summary> Минимально возможное значение для масштабирования. </summary>
        [SerializeField] private float _minPossibleValue = 100f;

        /// <summary> Максимально возможное значение для масштабирования. </summary>
        [SerializeField] private float _maxPossibleValue = 200f;

        /// <summary> Актуальное максимальное значение атрибута. </summary>
        private float _maxValue;

        /// <summary> Инициализировать отображение атрибута. </summary>
        /// <param name="currentValue"> Начальное текущее значение. </param>
        /// <param name="maxValue"> Начальное максимальное значение. </param>
        public override void Initialize(float currentValue, float maxValue)
        {
            _maxValue = maxValue;
            UpdateBarWidth(maxValue);
            UpdateVisual(currentValue, maxValue);
            UpdateTextVisibility(false);
        }

        /// <summary> Обновить отображение текущего значения. </summary>
        /// <param name="currentValue"> Новое значение. </param>
        /// <param name="delta"> Изменение значения. </param>
        public override void HandleAttributeChange(float currentValue, float delta)
        {
            UpdateCurrentValue(currentValue);
        }

        /// <summary> Обновить отображение, если значение достигло нуля. </summary>
        public override void HandleAttributeReachZero() { UpdateCurrentValue(0f); }

        /// <summary> Обновить отображение при изменении максимального значения. </summary>
        /// <param name="currentValue"> Актуальное текущее значение. </param>
        /// <param name="maxValue"> Новое максимальное значение. </param>
        public override void HandleAttributeMaxValueChanged(float currentValue, float maxValue)
        {
            _maxValue = maxValue;
            UpdateBarWidth(maxValue);
            _maxValueText.text = FormatValue(maxValue);
            UpdateCurrentValue(currentValue);
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

        /// <summary> Обновить визуальное представление значений. </summary>
        /// <param name="current"> Текущее значение. </param>
        /// <param name="max"> Максимальное значение. </param>
        private void UpdateVisual(float current, float max)
        {
            _maxValueText.text = FormatValue(max);
            UpdateCurrentValue(current);
        }

        /// <summary> Обновить отображение текущего значения и прогресс-бара. </summary>
        /// <param name="value"> Текущее значение. </param>
        private void UpdateCurrentValue(float value)
        {
            _fillImage.fillAmount = Mathf.Clamp01(value / Mathf.Max(_maxValue, 1f));
            _currentValueText.text = FormatValue(value);
        }

        /// <summary> Показать или скрыть текстовые значения. </summary>
        /// <param name="visible"> Показывать ли текст. </param>
        private void UpdateTextVisibility(bool visible)
        {
            _textParent.gameObject.SetActive(_alwaysShowValues || visible);
        }

        /// <summary> Обновить ширину полосы в зависимости от диапазона значений. </summary>
        /// <param name="currentMaxValue"> Текущее максимальное значение. </param>
        private void UpdateBarWidth(float currentMaxValue)
        {
            if (!_contentRect) return;

            float t = Mathf.InverseLerp(_minPossibleValue, _maxPossibleValue, currentMaxValue);
            float width = Mathf.Lerp(_baseBarWidth, _baseBarWidth * _maxScaleMultiplier, t);

            var size = _contentRect.sizeDelta;
            size.x = width;
            _contentRect.sizeDelta = size;
        }

        /// <summary> Преобразовать числовое значение в строку. </summary>
        /// <param name="value"> Значение. </param>
        /// <returns> Отформатированное целочисленное представление. </returns>
        private static string FormatValue(float value) => ((int)value).ToString();
    }
}