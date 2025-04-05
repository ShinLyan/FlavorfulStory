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
        /// <summary> Gameobject-контейнер количества предметов. </summary>
        [SerializeField] private GameObject _itemNumberContainer;

        /// <summary> Текст количества предметов. </summary>
        [SerializeField] private TMP_Text _itemNumberText;

        /// <summary> Изображение иконки предмета в инвентаре. </summary>
        private Image _iconImage;

        /// <summary> Снятие компонента <see cref="Image"/> текущего объекта
        /// в <see cref="_iconImage"/>. </summary>
        private void Awake() => _iconImage = GetComponent<Image>();

        /// <summary> Установить предмет инвентаря. </summary>
        /// <param name="item"> Предмет инвентаря. </param>
        /// <param name="number"> Количество предметов. </param>
        public void SetItem(InventoryItem item, int number)
        {
                _iconImage.enabled = item;
            if (item) _iconImage.sprite = item.Icon;

            _itemNumberContainer.SetActive(number > 1);
            _itemNumberText.text = number.ToString();
        }
    }
}