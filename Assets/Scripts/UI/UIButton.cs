using System;

namespace FlavorfulStory.UI
{
    /// <summary> Класс кнопки, которая реализует взаимодействие с UI. Наследуется от CustomButton. </summary>
    public class UIButton : CustomButton
    {
        /// <summary> Событие, которое вызывается при клике по кнопке. </summary>
        public event Action OnClick;

        /// <summary> Обработчик события клика по кнопке. Вызывает событие OnClick. </summary>
        protected override void Click()
        {
            base.Click();
            OnClick?.Invoke();
        }
    }
}