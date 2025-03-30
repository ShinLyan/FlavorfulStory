using System;

namespace FlavorfulStory.UI
{
    /// <summary> Класс кнопки, которая реализует взаимодействие с UI. Наследуется от CustomButton. </summary>
    public class UIButton : CustomButton
    {
        /// <summary> Событие, которое вызывается при клике по кнопке. </summary>
        public event Action OnClick;

        /// <summary> Разрешает взаимодействие с кнопкой. </summary>
        public void EnableInteraction() => IsInteractable = true;

        /// <summary> Блокирует взаимодействие с кнопкой. </summary>
        public void DisableInteraction() => IsInteractable = false;

        /// <summary> Вызывает клик по кнопке. </summary>
        public void TriggerClick() => Click();

        /// <summary> Удаляет все слушатели для события клика. </summary>
        public void RemoveAllListeners() => OnClick = null;

        /// <summary> Обработчик события клика по кнопке. Вызывает событие OnClick. </summary>
        protected override void Click() => OnClick?.Invoke();
    }
}