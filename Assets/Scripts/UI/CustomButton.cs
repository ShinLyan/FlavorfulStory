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
        /// <summary> Компонент <see cref="Image"/> кнопки. </summary>
        protected Image ButtonImage { get; private set; }

        /// <summary> Находится ли курсор над кнопкой? </summary>
        protected bool IsMouseOver { get; private set; }

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

        /// <summary> Вызывается при клике на кнопку. </summary>
        /// <param name="eventData"> Данные события клика. </param>
        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }

        /// <summary> Вызывается при наведении курсора на кнопку. </summary>
        /// <param name="eventData"> Данные события наведения. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsMouseOver = true;
            HoverStart();
        }

        /// <summary> Вызывается при уходе курсора с кнопки. </summary>
        /// <param name="eventData"> Данные события ухода курсора. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            IsMouseOver = false;
            HoverEnd();
        }
    }
}