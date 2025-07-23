using UnityEngine;
using TMPro;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Отображение всплывающей подсказки для предмета. </summary>
    public class ItemTooltipView : MonoBehaviour
    {
        /// <summary> Текстовое поле с названием предмета. </summary>
        [SerializeField] private TMP_Text _itemNameText;

        /// <summary> Текстовое поле с описанием предмета. </summary>
        [SerializeField] private TMP_Text _itemDescriptionText;

        /// <summary> Панель с иконкой золота, отображается, если предмет можно продать. </summary>
        [SerializeField] private GameObject _goldBar;

        /// <summary> Текстовое поле с ценой продажи предмета. </summary>
        [SerializeField] private TMP_Text _goldText;

        /// <summary> Настраивает отображение подсказки на основе переданного предмета. </summary>
        /// <param name="item"> Информация о предмете для отображения. </param>
        public void Setup(InventoryItem item)
        {
            _itemNameText.text = item.ItemName;
            _itemDescriptionText.text = item.Description;
            _goldBar.SetActive(item.IsSellable);
            if (item.IsSellable) _goldText.text = item.SellPrice.ToString();
        }
    }
}