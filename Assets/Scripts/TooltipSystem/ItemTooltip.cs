using TMPro;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.TooltipSystem
{
    /// <summary> Отображает всплывающую подсказку для предмета. </summary>
    public class ItemTooltip : MonoBehaviour
    {
        /// <summary> Текст заголовка подсказки. </summary>
        [SerializeField] private TMP_Text _titleText;

        /// <summary> Текст описания подсказки. </summary>
        [SerializeField] private TMP_Text _descriptionText;

        /// <summary> Устанавливает значения для отображения тултипа. </summary>
        /// <param name="title"> Текст заголовка. </param>
        /// <param name="description"> Текст описания. </param>
        public void Setup(string title, string description)
        {
            _titleText.text = title;
            _descriptionText.text = description;
        }
    }
}