using FlavorfulStory.Actions;
using FlavorfulStory.Crafting;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{ 
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/CraftingRecipe")]
    public class CraftingRecipeInventoryItem : InventoryItem, IUsable
    {
        /// <summary> Кнопка мыши для использования предмета. </summary>
        [field: Tooltip("Кнопка использования предмета."), SerializeField]
        public UseActionType UseActionType { get; private set; }
        
        [field: Tooltip("Соответствующий рецепт"), SerializeField]
        public CraftingRecipe CraftingRecipe { get; private set; }
        
        /// <summary> Использовать предмет для изучения рецепта крафта. </summary>
        /// <param name="player"></param>
        /// <param name="hitableLayers"></param>
        /// <returns></returns>
        public bool Use(PlayerController player, LayerMask hitableLayers)
        {
            player.RecipeUnlocker.Unlock(CraftingRecipe);

            return true;
        }
    }
}