using System;
using FlavorfulStory.Crafting;

namespace FlavorfulStory
{
    public interface ICraftingRecipeUnlocker
    {
        bool Unlock(string id);
        bool Unlock(CraftingRecipe recipe);
        event Action<CraftingRecipe> RecipeUnlocked;
    }
}