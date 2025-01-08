using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    [RequireComponent(typeof(Image))]
    public class TabSwitcherButton : MonoBehaviour,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _selectedSprite;

        [SerializeField] private UnityEvent _onSelected;

        /// <summary> Событие открытия вкладки. </summary>
        private Action _onClick;

        /// <summary> Свойство, используемое для нахождения соответсвующей клавиши в InputManager. </summary>
        [field: SerializeField] public ButtonType ButtonType { get; private set; }

        private Image _image;

        private bool _isSelected;
        
        /// <summary> Флаг нахождения курсора на кнопке. Типо как OnHover(). </summary>
        private bool _isMouseOver;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void Select()
        {
            _isSelected = true;
            _image.sprite = _selectedSprite;
            _onSelected?.Invoke();
        }

        public void ResetSelection()
        {
            _isSelected = false;
            if (!_isMouseOver) _image.sprite = _defaultSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isMouseOver = true;
            _image.sprite = _selectedSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseOver = false;
            if (!_isSelected) _image.sprite = _defaultSprite;
        }

        /// <summary> Метод добавления обработчика события _onClick. </summary>
        /// <param name="action"></param>
        public void AddOnClickHandler(Action action)
        {
            _onClick += action;
        }
    }
}