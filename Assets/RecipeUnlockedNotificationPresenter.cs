using System;
using Zenject;
using FlavorfulStory.Crafting;

namespace FlavorfulStory
{
    public class RecipeUnlockedNotificationPresenter : IInitializable, IDisposable
    {
        private readonly ICraftingRecipeUnlocker _recipeUnlocker;
        private readonly RecipeUnlockedNotificationWindow _window;

        public RecipeUnlockedNotificationPresenter(
            ICraftingRecipeUnlocker recipeUnlocker,
            RecipeUnlockedNotificationWindow window)
        {
            _recipeUnlocker = recipeUnlocker;
            _window = window;
        }

        public void Initialize() => _recipeUnlocker.RecipeUnlocked += OnRecipeUnlocked;

        public void Dispose() => _recipeUnlocker.RecipeUnlocked -= OnRecipeUnlocked;

        private void OnRecipeUnlocked(CraftingRecipe recipe)
        {
            _window.Setup(recipe.RecipeID);
            _window.Show();
        }
    }
}