using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.UI
{
    public class ToolbarSlotUI : CustomButton, IItemHolder
    {
        /// <summary> ������ ����� ���������. </summary>
        [SerializeField] private InventoryItemIcon _icon;

        /// <summary> ����� �������. </summary>
        [SerializeField] private TMP_Text _keyText;

        /// <summary> Изображение обводки тулбар слота. </summary>
        [SerializeField] private Image _hoverImage;
        
        const float FadeDuration = 0.2f;
        
        private int _index;
        
        private Toolbar _toolbar;
        
        private bool _isSelected;

        protected override void Initialize()
        {
            _index = transform.GetSiblingIndex();
            _keyText.text = $"{_index + 1}";
            _toolbar = transform.parent.GetComponent<Toolbar>();
        }

        protected override void HoverStart()
        {
            _hoverImage.CrossFadeAlpha(1.0f, FadeDuration, true);
        }

        protected override void HoverEnd()
        {
            if (!_isSelected)
            {
                _hoverImage.CrossFadeAlpha(0.0f, FadeDuration, true);
            }
        }

        protected override void Click()
        {
            _toolbar.SelectItem(_index);
        }
        
        public void Redraw() => _icon.SetItem(
            Inventory.PlayerInventory.GetItemInSlot(_index), 
            Inventory.PlayerInventory.GetNumberInSlot(_index)
            );

        /// <summary> �������� �������, ������� � ������ ������ ��������� � ���� ���������. </summary>
        /// <returns> ���������� �������, ������� � ������ ������ ��������� � ���� ���������. </returns>
        public InventoryItem GetItem() => Inventory.PlayerInventory.GetItemInSlot(_index);

        public void Select()
        {
            _isSelected = true;
            FadeToColor(Color.white);
            HoverStart();
        }

        public void ResetSelection()
        {
            _isSelected = false;
            FadeToColor(Color.gray);
            if (!IsMouseOver) HoverEnd();
        }
        private void FadeToColor(Color color)
        {
            ButtonImage.CrossFadeColor(color, FadeDuration, true, true);
        }
    }
}