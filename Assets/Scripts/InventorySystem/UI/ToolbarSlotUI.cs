using System;
using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.UI
{
    public class ToolbarSlotUI : CustomButton, IItemHolder
    {
        [SerializeField] private InventoryItemIcon _icon;
        
        [SerializeField] private TMP_Text _keyText;

        /// <summary> Изображение обводки тулбар слота. </summary>
        [SerializeField] private Image _hoverImage;
        
        const float FadeDuration = 0.2f;
        
        private int _index;
        
        private bool _isSelected;

        public event Action<int> OnSlotClicked; 
        
        protected override void Initialize()
        {
            _index = transform.GetSiblingIndex();
            _keyText.text = $"{_index + 1}";
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
            OnSlotClicked?.Invoke(_index);
        }
        
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
        
        public void Redraw() => _icon.SetItem(
            Inventory.PlayerInventory.GetItemInSlot(_index), 
            Inventory.PlayerInventory.GetNumberInSlot(_index)
            );
        
        public InventoryItem GetItem() => Inventory.PlayerInventory.GetItemInSlot(_index);
        
        private void FadeToColor(Color color)
        {
            ButtonImage.CrossFadeColor(color, FadeDuration, true, true);
        }
    }
}