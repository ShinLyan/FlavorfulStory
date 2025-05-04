using System.Globalization;
using FlavorfulStory.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory
{
    public class HoverAttributeView : BaseAttributeView, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Inspector refs")]
        [SerializeField] private RectTransform _contentRect;
        [SerializeField] private Image _fillImage;
        [SerializeField] private Transform _textParent;
        [SerializeField] private TMP_Text _currentValueText;
        [SerializeField] private TMP_Text _maxValueText;
        
        [Header("Settings")]
        [SerializeField] private bool _alwaysShowValues; 
        [SerializeField] private float _baseBarWidth = 200f;
        [SerializeField] private float _maxScaleMultiplier = 2f;
        [SerializeField] private float _minPossibleValue = 100f;
        [SerializeField] private float _maxPossibleValue = 200f;
        
        private float _maxValue;

        public override void InitializeView(float currentValue, float maxValue)
        {
            _maxValue = maxValue;
            UpdateBarWidth(maxValue);
            UpdateVisual(currentValue, maxValue);
            UpdateTextVisibility(false);
        }

        public override void HandleAttributeChange(float currentValue, float delta)
        {
            UpdateCurrentValue(currentValue);
        }

        public override void HandleAttributeReachZero()
        {
            UpdateCurrentValue(0f);
        }

        public override void HandleAttributeMaxValueChanged(float currentValue, float maxValue)
        {
            _maxValue = maxValue;
            UpdateBarWidth(maxValue);
            _maxValueText.text = FormatValue(maxValue);
            UpdateCurrentValue(currentValue);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_alwaysShowValues)
                UpdateTextVisibility(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_alwaysShowValues)
                UpdateTextVisibility(false);
        }

        private void UpdateVisual(float current, float max)
        {
            _maxValueText.text = FormatValue(max);
            UpdateCurrentValue(current);
        }

        private void UpdateCurrentValue(float value)
        {
            _fillImage.fillAmount = Mathf.Clamp01(value / Mathf.Max(_maxValue, 1f));
            _currentValueText.text = FormatValue(value);
        }

        private void UpdateTextVisibility(bool visible)
        {
            _textParent.gameObject.SetActive(_alwaysShowValues || visible);
        }

        private void UpdateBarWidth(float currentMaxValue)
        {
            if (_contentRect == null) return;

            float t = Mathf.InverseLerp(_minPossibleValue, _maxPossibleValue, currentMaxValue);
            float width = Mathf.Lerp(_baseBarWidth, _baseBarWidth * _maxScaleMultiplier, t);

            var size = _contentRect.sizeDelta;
            size.x = width;
            _contentRect.sizeDelta = size;
        }

        protected virtual string FormatValue(float value)
        {
            // Ну пздец
            return ((int) value).ToString();
        }
    }
}