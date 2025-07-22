using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Отображение стака предмета. </summary>
    [RequireComponent(typeof(Image))]
    public class ItemStackView : MonoBehaviour
    {
        /// <summary> Иконка предмета. </summary>
        [SerializeField] private Image _itemImage;

        /// <summary> Контейнер текста количества предметов. </summary>
        [SerializeField] private GameObject _itemNumberContainer;

        /// <summary> Текст количества предметов. </summary>
        [SerializeField] private TMP_Text _itemNumberText;

        /// <summary> Обновить отображение стака предмета. </summary>
        /// <param name="itemStack"> Предмет и его количество. </param>
        public void UpdateView(ItemStack itemStack)
        {
            _itemImage.enabled = itemStack.Item;
            if (itemStack.Item) _itemImage.sprite = itemStack.Item.Icon;

            _itemNumberContainer.SetActive(itemStack.Number > 1);
            _itemNumberText.text = itemStack.Number.ToString();
        }
    }
}