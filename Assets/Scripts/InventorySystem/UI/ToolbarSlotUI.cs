using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.UI
{
    [RequireComponent(typeof(Image))]
    public class ToolbarSlotUI : MonoBehaviour, IItemHolder,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> ������ ����� ���������. </summary>
        [SerializeField] private InventoryItemIcon _icon;

        /// <summary> ����� �������. </summary>
        [SerializeField] private TMP_Text _keyText;

        /// <summary> ������ ����� � HUD. </summary>
        private int _index;

        /// <summary>
        /// 
        /// </summary>
        private Toolbar _toolbar;

        /// <summary>
        /// 
        /// </summary>
        private bool _isSelected;

        private void Awake()
        {
            _index = transform.GetSiblingIndex();
            _keyText.text = $"{_index + 1}";
            _toolbar = transform.parent.GetComponent<Toolbar>();
        }

        public void Redraw()
        {
            _icon.SetItem(Inventory.PlayerInventory.GetItemInSlot(_index), 
                Inventory.PlayerInventory.GetNumberInSlot(_index));
        }

        /// <summary> �������� �������, ������� � ������ ������ ��������� � ���� ���������. </summary>
        /// <returns> ���������� �������, ������� � ������ ������ ��������� � ���� ���������. </returns>
        public InventoryItem GetItem() => Inventory.PlayerInventory.GetItemInSlot(_index);

        public void Select()
        {
            _isSelected = true;
            FadeToColor(Color.white);
        }

        public void ResetSelection()
        {
            _isSelected = false;
            FadeToColor(Color.gray);
        }
        private void FadeToColor(Color color)
        {
            const float FadeDuration = 0.2f;
            GetComponent<Image>().CrossFadeColor(color, FadeDuration, true, true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _toolbar.SelectItem(_index);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            FadeToColor(Color.white);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //if (!_isSelected) FadeToColor(Color.white);
            if (!_isSelected) FadeToColor(Color.gray);
        }
    }
}