using System;
using FlavorfulStory.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory
{
    public class CraftRecipeView : CustomButton
    {
        /// <summary> Альфа для заблокированного крафта. </summary>
        [SerializeField] private float _lockedItemAlpha = 0.125f;
        /// <summary> Изображение предмета. </summary>
        [SerializeField] private Image _itemImage;
        /// <summary> Изображение замка. </summary>
        [SerializeField] private Image _lockImage;

        private Action _clickAction;
        
        //TODO: Передать Action кнопке - выбор рецепта крафта
        public void Setup(bool isLocked, Sprite sprite, Action clickAction)
        {
            float alpha = isLocked ? _lockedItemAlpha : 1f;
            Color color = new(1, 1, 1, alpha);
            _itemImage.color = color;
            _lockImage.enabled = isLocked;
            _clickAction = clickAction;
            OnClick += _clickAction;
            _itemImage.sprite = sprite;
        }
        
        private void OnDisable() => OnClick -= _clickAction;
        
        public void Disable()
        {
            transform.parent.gameObject.SetActive(false);
            OnClick -= _clickAction;
        }

        public void Enable()
        {
            transform.parent.gameObject.SetActive(true);
        }
    }
}