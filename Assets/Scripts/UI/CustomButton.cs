using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    /// <summary>
    /// Абстрактный класс, реализующий поведение пользовательской кнопки с обработкой наведения и кликов мыши.
    /// Класс требует наличия компонента Image.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public abstract class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Свойство для доступа к компоненту Image кнопки. </summary>
        protected Image ButtonImage { get; private set; }
        
        /// <summary> Флаг, указывающий, что мышь находится над кнопкой. </summary>
        protected bool IsMouseOver { get; private set; }

        /// <summary> Вызывается при деактивации компонента. Сбрасывает состояние IsMouseOver. </summary>
        private void OnDisable()
        {
            IsMouseOver = false;
        }

        /// <summary> Инициализация кнопки. Получает компонент Image и вызывает метод Initialize(). </summary>
        protected virtual void Awake()
        {
            ButtonImage = GetComponent<Image>();
            Initialize();
        }

        /// <summary> Абстрактный метод инициализации, который должен быть реализован в дочернем классе. </summary>
        protected abstract void Initialize();

        /// <summary> Абстрактный метод, вызываемый при начале наведения мыши на кнопку. </summary>
        protected abstract void HoverStart();

        /// <summary> Абстрактный метод, вызываемый при завершении наведения мыши на кнопку. </summary>
        protected abstract void HoverEnd();

        /// <summary> Абстрактный метод, вызываемый при клике на кнопку. </summary>
        protected abstract void Click();

        /// <summary> Обрабатывает событие клика по кнопке. </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }

        /// <summary> Обрабатывает событие наведения мыши на кнопку. </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsMouseOver = true;
            HoverStart();
        }

        /// <summary> Обрабатывает событие ухода мыши с кнопки. </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            IsMouseOver = false;
            HoverEnd();
        }
    }
}