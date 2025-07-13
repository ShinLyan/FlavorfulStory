using UnityEngine;
using FlavorfulStory.InventorySystem.UI.Tooltips;

namespace FlavorfulStory.Crafting
{
    /// <summary> Спавнит тултип с информацией о рецепте. </summary>
    [RequireComponent(typeof(IRecipeHolder))]
    public class RecipeTooltipSpawner : TooltipSpawner
    {
        /// <summary> Можно ли создать тултип? </summary>
        protected override bool CanCreateTooltip() =>
            GetComponent<IRecipeHolder>().GetRecipe() != null;

        /// <summary> Обновление содержимого тултипа. </summary>
        protected override void UpdateTooltip(GameObject tooltip)
        {
            if (!tooltip.TryGetComponent<RecipeTooltip>(out var recipeTooltip)) return;

            var recipe = GetComponent<IRecipeHolder>().GetRecipe();
            recipeTooltip.Setup(recipe.RecipeName, BuildDescription(recipe));
        }

        /// <summary> Собирает текст описания рецепта. </summary>
        private string BuildDescription(CraftingRecipe recipe)
        {
            return recipe.Description;
        }
    }
}