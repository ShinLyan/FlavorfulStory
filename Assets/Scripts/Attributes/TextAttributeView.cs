using System.Globalization;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Attributes
{
    /// <summary> Представление атрибута с отображением значения через текстовый компонент. </summary>
    public class TextAttributeView : BaseAttributeView
    {
        /// <summary> Текстовый компонент для отображения значения атрибута. </summary>
        [SerializeField] private TMP_Text _text;

        /// <summary> Обновляет текст при изменении значения атрибута. </summary>
        /// <param name="currentValue"> Текущее значение атрибута. </param>
        /// <param name="delta"> Изменение значения. </param>
        public override void HandleAttributeChange(float currentValue, float delta)
        {
            _text.text = $"{currentValue:F2}";
        }

        /// <summary> Отображает сообщение при достижении нулевого значения. </summary>
        public override void HandleAttributeReachZero()
        {
            _text.text = "DEAD";
            print("Ебать ты лох! Сдох, ахахаха");
        }

        /// <summary> Реагирует на изменение максимального значения атрибута. </summary>
        /// <param name="currentValue"> Текущее значение атрибута. </param>
        /// <param name="maxValue"> Новое максимальное значение. </param>
        public override void HandleAttributeMaxValueChanged(float currentValue, float maxValue)
        {
            print("Нихуя! Ты че подкачался, эй");
        }

        /// <summary> Инициализирует текст начальным значением атрибута. </summary>
        /// <param name="currentValue"> Начальное текущее значение. </param>
        /// <param name="maxValue"> Начальное максимальное значение. </param>
        public override void InitializeView(float currentValue, float maxValue)
        {
            _text.text = currentValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}