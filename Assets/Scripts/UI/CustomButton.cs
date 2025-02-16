using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    /// <summary> Базовый класс пользовательской кнопки с обработкой событий наведения и кликов. </summary>
    /// <remarks> Требует наличия компонента <see cref="Image"/>. </remarks>
    [RequireComponent(typeof(Image))]
    public abstract class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private readonly Color _interactionDisabledColor = Color.gray;
        private readonly Color _interactionEnabledColor = Color.white;
        
        /// <summary> Компонент <see cref="Image"/> кнопки. </summary>
        protected Image ButtonImage { get; private set; }

        /// <summary> Находится ли курсор над кнопкой? </summary>
        protected bool IsMouseOver { get; private set; }

        private bool _isInteractable = true;
        public bool IsInteractable
        {
            get => _isInteractable;
            set
            {
                _isInteractable = value;
                if (_isInteractable)
                {
                    ButtonImage.CrossFadeColor(_interactionEnabledColor, 0.01f, true, false);
                    OnInteractionEnabled();
                }
                else
                {
                    ButtonImage.CrossFadeColor(_interactionDisabledColor, 0.01f, true, false);
                    IsMouseOver = false;
                    HoverEnd();
                    OnInteractionDisabled();
                }
            }
        }

        /// <summary> Сброс состояния наведения при деактивации компонента. </summary>
        private void OnDisable()
        {
            IsMouseOver = false;
        }

        /// <summary> Инициализация кнопки, получение компонента <see cref="Image"/>. </summary>
        protected virtual void Awake()
        {
            ButtonImage = GetComponent<Image>();
            Initialize();
        }

        /// <summary> Метод инициализации, который должен быть реализован в дочерних классах. </summary>
        protected abstract void Initialize();

        /// <summary> Действие при наведении курсора на кнопку. </summary>
        protected abstract void HoverStart();

        /// <summary> Действие при уходе курсора с кнопки. </summary>
        protected abstract void HoverEnd();

        /// <summary> Действие при клике на кнопку. </summary>
        protected abstract void Click();

        protected abstract void OnInteractionEnabled();
        
        protected abstract void OnInteractionDisabled();
        
        /// <summary> Вызывается при клике на кнопку. </summary>
        /// <param name="eventData"> Данные события клика. </param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsInteractable) return;
            
            Click();
        }

        /// <summary> Вызывается при наведении курсора на кнопку. </summary>
        /// <param name="eventData"> Данные события наведения. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInteractable) return;
            
            IsMouseOver = true;
            HoverStart();
        }

        /// <summary> Вызывается при уходе курсора с кнопки. </summary>
        /// <param name="eventData"> Данные события ухода курсора. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsInteractable) return;
            
            IsMouseOver = false;
            HoverEnd();
        }
    }
}