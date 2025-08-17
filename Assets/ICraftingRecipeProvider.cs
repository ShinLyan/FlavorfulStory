using System;
using System.Collections.Generic;
using FlavorfulStory.Crafting;

namespace FlavorfulStory
{
    public interface ICraftingRecipeProvider
    {
        IReadOnlyList<CraftingRecipe> All { get; }
        CraftingRecipe GetById(string id);
        bool IsUnlocked(string id);
    }
}