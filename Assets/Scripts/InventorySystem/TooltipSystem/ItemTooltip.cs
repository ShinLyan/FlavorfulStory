using TMPro;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.TooltipSystem
{
    /// <summary> Тултип (всплывающая подсказка) предмета.</summary>
    public class ItemTooltip : MonoBehaviour
    {
        /// <summary> Текст заголовка.</summary>
        [SerializeField] private TMP_Text _titleText;

        /// <summary> Текст описания.</summary>
        [SerializeField] private TMP_Text _descriptionText;

        /// <summary> Установка значений тултип.</summary>
        /// <param name="item"></param>
        public void Setup(InventoryItem item)
        {
            _titleText.text = item.ItemName;
            _descriptionText.text = item.Description;
        }
    }
}