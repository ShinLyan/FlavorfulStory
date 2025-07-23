using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.Crafting
{
    /// <summary> UI-элемент, отображающий требуемый предмет для крафта. </summary>
    public class CraftRequirementView : MonoBehaviour, IItemHolder
    {
        /// <summary> Цвет текста, если требование выполнено. </summary>
        [SerializeField] private Color _requirementAchievedTextColor;
        /// <summary> Цвет текста, если требование не выполнено. </summary>
        [SerializeField] private Color _requirementFailedTextColor;
        
        /// <summary> Изображение предмета. </summary>
        [SerializeField] private Image _itemImage;
        /// <summary> Название предмета. </summary>
        [SerializeField] private TMP_Text _itemName;
        /// <summary> Текстовое отображение количества и состояния требования. </summary>
        [SerializeField] private TMP_Text _requirementText;

        /// <summary> ID требуемого предмета. </summary>
        private string _itemID;
        
        /// <summary> Настраивает отображение требования по предмету. </summary>
        /// <param name="itemID"> ID предмета из базы данных. </param>
        /// <param name="requirementText"> Текстовое описание требования (например, "2/3"). </param>
        /// <param name="requirementAchieved"> True, если требование выполнено. </param>
        public void Setup(string itemID, string requirementText, bool requirementAchieved)
        {
            _itemID = itemID;
            _itemImage.sprite = ItemDatabase.GetItemFromID(_itemID).Icon;
            _itemName.text = ItemDatabase.GetItemFromID(_itemID).ItemName;
            _requirementText.color =requirementAchieved ? _requirementAchievedTextColor : _requirementFailedTextColor;
            _requirementText.text = requirementText;
        }

        /// <summary> Возвращает предмет, связанный с данным UI-элементом. </summary>
        public InventoryItem GetItem() => ItemDatabase.GetItemFromID(_itemID);
    }
}