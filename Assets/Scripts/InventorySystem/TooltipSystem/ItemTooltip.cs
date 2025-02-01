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
        /// <param name="item"> Предмет, данные которого будут отображены. </param>
        public void Setup(InventoryItem item)
        {
            _titleText.text = item.ItemName;
            _descriptionText.text = item.Description;
        }
    }
}