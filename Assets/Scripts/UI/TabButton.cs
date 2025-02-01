using System;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Кнопка вкладки, наследующая от CustomButton. 
    /// Управляет отображением текста и состоянием вкладки. </summary>
    public class TabButton : CustomButton
    {
        /// <summary> Текст на кнопке вкладки. </summary>
        [SerializeField] private TMP_Text _label;

        /// <summary> Цвет текста при активной вкладке. </summary>
        [SerializeField] private Color _activeLabelColor;

        /// <summary> Цвет текста при неактивной вкладке. </summary>
        [SerializeField] private Color _defaultLabelColor;

        /// <summary> Активна ли вкладка? </summary>
        public bool IsActive { get; set; }

        /// <summary> Событие клика по вкладке. </summary>
        public event Action OnClick;

        /// <summary> Установка цвета текста при активации компонента. </summary>
        private void OnEnable()
        {
            _label.color = IsActive ? _activeLabelColor : _defaultLabelColor;
        }

        /// <summary> Инициализация кнопки. </summary>
        protected override void Initialize()
        {
            SetNameState(false);
        }

        /// <summary> Наведение курсора на кнопку. </summary>
        protected override void HoverStart()
        {
            SetNameState(true);
        }

        /// <summary> Уход курсора с кнопки. </summary>
        protected override void HoverEnd()
        {
            if (!IsActive) SetNameState(false);
        }

        /// <summary> Клик по кнопке. </summary>
        protected override void Click()
        {
            SetNameState(true);
            OnClick?.Invoke();
        }

        /// <summary> Установка состояния текста (активное или стандартное). </summary>
        /// <param name="state"> Состояние текста (true - активное, false - стандартное). </param>
        public void SetNameState(bool state)
        {
            _label.color = state || IsMouseOver ? _activeLabelColor : _defaultLabelColor;
        }
    }
}