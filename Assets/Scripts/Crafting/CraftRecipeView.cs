using System;
using UnityEngine;
using UnityEngine.UI;
using FlavorfulStory.UI;
using Zenject;

namespace FlavorfulStory.Crafting
{
    /// <summary> Отображает кнопку рецепта крафта с иконкой, замком и взаимодействием. </summary>
    public class CraftRecipeView : CustomButton, IRecipeHolder
    {
        /// <summary> Иконка предмета рецепта. </summary>
        [SerializeField] private Image _itemImage;  
        /// <summary> Изображение замка, отображающее блокировку рецепта. </summary>
        [SerializeField] private Image _lockImage;

        /// <summary> ID рецепта, связанного с этим элементом. </summary>
        private CraftingRecipe _recipe;
        /// <summary> Действие, вызываемое при клике на элемент. </summary>
        private Action _clickAction;
        //TODO: комм
        private ICraftingRecipeProvider _provider;
        
        /// <summary> Альфа-прозрачность для заблокированного рецепта. </summary>
        private const float LockedItemAlpha = 0.125f;

        [Inject]
        private void Construct(ICraftingRecipeProvider provider) => _provider = provider;
        
        /// <summary> Настраивает отображение рецепта (иконка, блокировка, колбэк). </summary>
        /// <param name="recipeID"> ID рецепта. </param>
        /// <param name="clickAction"> Действие по клику. </param>
        public void Setup(string recipeID, Action clickAction)
        {
            if (_clickAction != null) OnClick -= _clickAction;
            _clickAction = clickAction;
            OnClick += _clickAction;

            _recipe = _provider.GetById(recipeID);
            var isUnlocked = _provider.IsUnlocked(recipeID);

            _itemImage.sprite = _recipe.Sprite;
            _itemImage.color = new Color(1f, 1f, 1f, isUnlocked ? 1f : LockedItemAlpha);
            _lockImage.enabled = !isUnlocked;
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

        /// <summary> Получить рецепт. </summary>
        /// <returns> Выбранный рецепт в <see cref="CraftingWindow"/>. </returns>
        public CraftingRecipe GetRecipe() => _recipe;
    }
}