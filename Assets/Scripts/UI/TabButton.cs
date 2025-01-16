using System;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Реализация кнопки вкладки, наследующая от CustomButton.
    /// Обрабатывает логику отображения текста и активность вкладки. </summary>
    public class TabButton : CustomButton
    {
        /// <summary> Текст, отображаемый на кнопке вкладки. </summary>
        [SerializeField] private TMP_Text _label;

        /// <summary> Цвет текста, когда вкладка активна. </summary>
        [SerializeField] private Color _activeLabelColor;

        /// <summary> Цвет текста, когда вкладка неактивна. </summary>
        [SerializeField] private Color _defaultLabelColor;
        
        /// <summary> Флаг активности вкладки. </summary>
        public bool IsActive { get; set; }

        /// <summary> Событие клика по кнопке вкладки. </summary>
        public event Action OnClick;

        /// <summary> При активации компонента устанавливает цвет текста в зависимости от состояния вкладки. </summary>
        private void OnEnable()
        {
            _label.color = IsActive ? _activeLabelColor : _defaultLabelColor;
        }
        
        /// <summary> Инициализация кнопки вкладки. Устанавливает начальное состояние цвета текста. </summary>
        protected override void Initialize()
        {
            SetNameState(false);
        }

        /// <summary> Обрабатывает начало наведения мыши на кнопку вкладки. </summary>
        protected override void HoverStart()
        {
            SetNameState(true);
        }

        /// <summary> Обрабатывает окончание наведения мыши на кнопку вкладки. </summary>
        protected override void HoverEnd()
        {
            if (!IsActive) SetNameState(false);
        }

        /// <summary> Обрабатывает клик по кнопке вкладки. </summary>
        protected override void Click()
        {
            SetNameState(true);
            OnClick?.Invoke();
        }

        /// <summary> Устанавливает состояние текста кнопки (активный или обычный цвет). </summary>
        public void SetNameState(bool state)
        {
            _label.color = state || IsMouseOver ? _activeLabelColor : _defaultLabelColor;
        }
    }
}