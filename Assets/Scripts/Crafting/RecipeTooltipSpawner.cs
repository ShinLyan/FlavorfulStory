using UnityEngine;
using Zenject;
using FlavorfulStory.TooltipSystem;

namespace FlavorfulStory.Crafting
{
    /// <summary> Спавнит тултип с информацией о рецепте. </summary>
    [RequireComponent(typeof(IRecipeHolder))]
    public class RecipeTooltipSpawner : TooltipSpawner
    {
        /// <summary> Ссылка на компонент, содержащий рецепт. </summary>
        private IRecipeHolder _recipeHolder;

        /// <summary> Инжектирует префаб тултипа рецепта. </summary>
        /// <param name="tooltipPrefab"> Префаб тултипа. </param>
        [Inject]
        private void Construct(RecipeTooltipView tooltipPrefab) =>
            TooltipPrefab = tooltipPrefab.gameObject;

        /// <summary> Получает ссылку на компонент рецепта при инициализации. </summary>
        private void Awake() => _recipeHolder = GetComponent<IRecipeHolder>();

        /// <summary> Проверяет, можно ли создать тултип (если есть рецепт). </summary>
        /// <returns> Существует ли рецепт?. </returns>
        protected override bool CanCreateTooltip() => _recipeHolder?.GetRecipe() != null;

        /// <summary> Обновляет содержимое тултипа на основе текущего рецепта. </summary>
        /// <param name="tooltip"> Тултип, который нужно обновить. </param>
        protected override void UpdateTooltip(GameObject tooltip)
        {
            if (!tooltip.TryGetComponent<RecipeTooltipView>(out var recipeTooltip)) return;

            var recipe = _recipeHolder.GetRecipe();
            recipeTooltip.Setup(recipe.RecipeName, BuildDescription(recipe));
        }

        /// <summary> Формирует описание рецепта. </summary>
        /// <param name="recipe"> Рецепт для отображения. </param>
        /// <returns> Строку описания. </returns>
        private string BuildDescription(CraftingRecipe recipe) => recipe.Description;
    }
}