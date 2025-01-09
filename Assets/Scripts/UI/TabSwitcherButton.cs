using System;
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
        /// <summary> Спрайт, отображаемый кнопкой по умолчанию. </summary>
        [SerializeField] private Sprite _defaultSprite;

        /// <summary> Спрайт, отображаемый кнопкой при выборе. </summary>
        [SerializeField] private Sprite _selectedSprite;

        /// <summary> Unity-событие, вызываемое при выборе кнопки. </summary>
        [SerializeField] private UnityEvent _onSelected;

        /// <summary> Тип кнопки, используемый для идентификации в InputManager. </summary>
        [field: SerializeField] public ButtonType ButtonType { get; private set; }

        /// <summary> Ссылка на компонент Image, связанный с кнопкой. </summary>
        private Image _image;

        /// <summary> Флаг, указывающий, выбрана ли кнопка. </summary>
        private bool _isSelected;

        /// <summary> Флаг нахождения курсора на кнопке. Типо как OnHover(). </summary>
        private bool _isMouseOver;

        /// <summary> Событие клика по кнопке, вызываемое внешними обработчиками. </summary>
        private Action _onClick;

        /// <summary> Метод, выполняемый при инициализации объекта. Устанавливает ссылку на компонент Image. </summary>
        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        /// <summary> Выбор кнопки. Устанавливает флаг выбора, изменяет спрайт и вызывает событие _onSelected. </summary>
        public void Select()
        {
            _isSelected = true;
            _image.sprite = _selectedSprite;
            _onSelected?.Invoke();
        }

        /// <summary> Сброс выбора кнопки. Убирает спрайт выбора, если указатель мыши не находится над кнопкой. </summary>
        public void ResetSelection()
        {
            _isSelected = false;
            if (!_isMouseOver) _image.sprite = _defaultSprite;
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
            _image.sprite = _selectedSprite;
        }

        /// <summary> Обработчик выхода указателя за пределы кнопки. Возвращает спрайт в состояние по умолчанию, если кнопка не выбрана. </summary>
        /// <param name="eventData"> Данные события указателя. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseOver = false;
            if (!_isSelected) _image.sprite = _defaultSprite;
        }

        /// <summary> Метод добавления обработчика для события _onClick. </summary>
        /// <param name="action"> Действие, которое будет вызвано при клике. </param>
        public void AddOnClickHandler(Action action)
        {
            _onClick += action;
        }
    }
}