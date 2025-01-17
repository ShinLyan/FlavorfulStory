using System;
using System.Collections;
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

        private Color _defaultColor; 
        private Color _selectedColor = new Color(1.0f, 1.0f, 1.0f, 0.35f); 
        
        /// <summary> Изображение обводки тулбар слота. </summary>
        [SerializeField] private Image _hoverImage;
        
        const float FadeDuration = 0.1f;
        
        private int _index;
        
        private bool _isSelected;

        public event Action<int> OnSlotClicked; 
        
        protected override void Initialize()
        {
            _index = transform.GetSiblingIndex();
            _keyText.text = $"{_index + 1}";
            _defaultColor = ButtonImage.color;
        }

        protected override void HoverStart()
        {
            _hoverImage.CrossFadeAlpha(1.0f, FadeDuration, true);
        }

        protected override void HoverEnd()
        {
            _hoverImage.CrossFadeAlpha(0.0f, FadeDuration, true);
        }

        protected override void Click()
        {
            OnSlotClicked?.Invoke(_index);
        }
        
        public void Select()
        {
            _isSelected = true;
            FadeToColor(_selectedColor);
            //HoverStart();
        }

        public void ResetSelection()
        {
            _isSelected = false;
            FadeToColor(_defaultColor);
            if (!IsMouseOver) HoverEnd();
        }
        
        public void Redraw() => _icon.SetItem(
            Inventory.PlayerInventory.GetItemInSlot(_index), 
            Inventory.PlayerInventory.GetNumberInSlot(_index)
            );
        
        public InventoryItem GetItem() => Inventory.PlayerInventory.GetItemInSlot(_index);
        
        private void FadeToColor(Color color)
        {
            //ButtonImage.CrossFadeColor(color, FadeDuration, true, true);
            StartCoroutine(FadeToColorCoroutine(color));
        }

        private IEnumerator FadeToColorCoroutine(Color color)
        {
            var startColor = ButtonImage.color;
            float timeElapsed = 0;
            while (timeElapsed <= FadeDuration)
            {
                ButtonImage.color = Color.Lerp(startColor, color, timeElapsed / FadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            ButtonImage.color = color;
        }
    }
}