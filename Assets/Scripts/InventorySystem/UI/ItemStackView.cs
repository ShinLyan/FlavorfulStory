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
        /// <param name="item"> Предмет инвентаря. </param>
        /// <param name="number"> Количество предметов. </param>
        public void UpdateView(InventoryItem item, int number)
        {
            _itemImage.enabled = item;
            if (item) _itemImage.sprite = item.Icon;

            _itemNumberContainer.SetActive(number > 1);
            _itemNumberText.text = number.ToString();
        }
    }
}