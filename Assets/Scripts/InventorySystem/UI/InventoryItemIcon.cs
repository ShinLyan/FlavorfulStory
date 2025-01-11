using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Иконка предмета - отображает иконку предмета
    /// и его текущее количество в слоте инвентаря. </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        /// <summary> Контейнер для текста количества предметов. </summary>
        [SerializeField] private GameObject _textContainer;

        /// <summary> Текст количества предметов. </summary>
        [SerializeField] private TMP_Text _itemNumberText;

        /// <summary> Установить предмет инвентаря. </summary>
        /// <param name="item"> Предмет инвентаря. </param>
        /// <param name="number"> Количество предметов. </param>
        public void SetItem(InventoryItem item, int number)
        {
            var iconImage = GetComponent<Image>();
            iconImage.enabled = item != null;
            if (item) iconImage.sprite = item.Icon;

            if (!_itemNumberText) return;

            _textContainer.SetActive(number > 1);
            _itemNumberText.text = number.ToString();
        }
    }
}