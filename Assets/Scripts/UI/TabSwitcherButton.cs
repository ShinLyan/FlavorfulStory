using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    /// <summary> Кнопка переключения вкладки. </summary>
    [RequireComponent(typeof(Image))]
    public class TabSwitcherButton : MonoBehaviour,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Текстовое поле названия вкладки. </summary>
        [SerializeField] private TMP_Text _tabNameText;

        /// <summary> Unity-событие, вызываемое при выборе кнопки. </summary>
        [SerializeField] private UnityEvent _onSelected;

        /// <summary> Тип кнопки, используемый для идентификации в InputManager. </summary>
        [field: SerializeField] public ButtonType ButtonType { get; private set; }

        /// <summary> Цвет текста выбранной вкладки. </summary>
        private Color _selectedColor = new(1, 0.8566375f, 0.6745283f);

        /// <summary> Цвет текста вкладки по умолчанию. </summary>
        private Color _defaultColor = Color.white;

        /// <summary> Флаг, указывающий, выбрана ли кнопка. </summary>
        private bool _isSelected;

        /// <summary> Флаг нахождения курсора на кнопке. Типо как OnHover(). </summary>
        private bool _isMouseOver;

        /// <summary> Событие клика по кнопке, вызываемое внешними обработчиками. </summary>
        private Action _onClick;

        /// <summary> Выбор кнопки. Устанавливает флаг выбора, изменяет спрайт и вызывает событие _onSelected. </summary>
        public void Select()
        {
            _isSelected = true;
            _tabNameText.color = _selectedColor;
            _onSelected?.Invoke();
        }

        /// <summary> Сброс выбора кнопки. Убирает спрайт выбора, если указатель мыши не находится над кнопкой. </summary>
        public void ResetSelection()
        {
            _isSelected = false;
            if (!_isMouseOver) _tabNameText.color = _defaultColor;
        }

        /// <summary> Обработчик клика по кнопке. Вызывает событие _onClick. </summary>
        /// <param name="eventData"> Данные события указателя. </param>
        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }

        /// <summary> Обработчик наведения указателя на кнопку. Устанавливает спрайт выбора. </summary>
        /// <param name="eventData"> Данные события указателя. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isMouseOver = true;
            _tabNameText.color = _selectedColor;
        }

        /// <summary> Обработчик выхода указателя за пределы кнопки. Возвращает спрайт в состояние по умолчанию, если кнопка не выбрана. </summary>
        /// <param name="eventData"> Данные события указателя. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseOver = false;
            if (!_isSelected) _tabNameText.color = _defaultColor;
        }

        /// <summary> Метод добавления обработчика для события _onClick. </summary>
        /// <param name="action"> Действие, которое будет вызвано при клике. </param>
        public void AddOnClickHandler(Action action)
        {
            _onClick += action;
        }
    }
}