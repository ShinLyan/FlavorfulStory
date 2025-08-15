using TMPro;
using UnityEngine;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Отображение всплывающей подсказки для кнопки. </summary>
    public class ButtonTooltipView : MonoBehaviour
    {
        /// <summary> Текстовое поле с названием кнопки. </summary>
        [SerializeField] private TMP_Text _buttonInfoText;

        /// <summary> Настраивает отображение подсказки на основе кнопки. </summary>
        /// <param name="info"> Информация о кнопке. </param>
        public void Setup(string info) => _buttonInfoText.text = info;
    }
}