using System;
using FlavorfulStory.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.Crafting
{
    /// <summary> Отображает кнопку рецепта крафта с иконкой, замком и взаимодействием. </summary>
    public class CraftRecipeView : CustomButton
    {
        /// <summary> Иконка предмета рецепта. </summary>
        [SerializeField] private Image _itemImage;

        /// <summary> Изображение замка, отображающее блокировку рецепта. </summary>
        [SerializeField] private Image _lockImage;

        /// <summary>
        /// 
        /// </summary>
        private Action _clickAction;

        /// <summary> Альфа-прозрачность для заблокированного рецепта. </summary>
        private const float LockedItemAlpha = 0.125f;

        /// <summary> Настраивает отображение рецепта (иконка, блокировка, колбэк). </summary>
        /// <param name="isLocked"> Заблокирован ли рецепт. </param>
        /// <param name="sprite"> Спрайт рецепта. </param>
        /// <param name="clickAction"> Действие по клику. </param>
        public void Setup(bool isLocked, Sprite sprite, Action clickAction)
        {
            if (_clickAction != null) OnClick -= _clickAction;

            _clickAction = clickAction;
            OnClick += _clickAction;

            _itemImage.sprite = sprite;
            _itemImage.color = new Color(1f, 1f, 1f, isLocked ? LockedItemAlpha : 1f);
            _lockImage.enabled = isLocked;
        }

        /// <summary> Вызывается при отключении объекта. Отписывает колбэк. </summary>
        private void OnDisable()
        {
            if (_clickAction != null) OnClick -= _clickAction;
        }

        /// <summary> Отключает отображение рецепта. </summary>
        public void Disable()
        {
            if (_clickAction != null) OnClick -= _clickAction;

            _itemImage.sprite = null;
            transform.parent.gameObject.SetActive(false);
        }

        /// <summary> Включает отображение рецепта. </summary>
        public void Enable() => transform.parent.gameObject.SetActive(true);
    }
}