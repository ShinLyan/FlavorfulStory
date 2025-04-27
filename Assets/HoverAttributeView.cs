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
        [SerializeField] private Transform _textParent;
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _currentValueText;
        [SerializeField] private TMP_Text _maxValueText;

        //TODO: МБ будет лучше инвокать HandleAttributeMaxValueChanged(float currentValue),
        //где currentValue ограничить диапазоном [0; 1f]
        private float _maxValue;

        public override void HandleAttributeChange(float currentValue, float delta)
        {
            //Специфический формат отображения устанавливать тут
            _fillImage.fillAmount = currentValue / _maxValue;
            _currentValueText.text = currentValue.ToString(CultureInfo.InvariantCulture);
        }

        public override void HandleAttributeReachZero()
        {
            //Специфический формат отображения устанавливать тут
            _fillImage.fillAmount = 0f;
            _currentValueText.text = "0";
        }

        public override void HandleAttributeMaxValueChanged(float currentValue, float maxValue)
        {
            _maxValueText.text = maxValue.ToString(CultureInfo.InvariantCulture);
            _maxValue = maxValue;
        }

        public override void InitializeView(float currentValue, float maxValue)
        {
            _textParent.gameObject.SetActive(false);
            _maxValue = maxValue;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _textParent.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _textParent.gameObject.SetActive(false);
        }
    }
}