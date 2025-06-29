using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.CraftingSystem
{
    public static class CraftingManager
    {
        private static Dictionary<string, CraftRecipe> _recipes;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            _recipes = new();
            foreach (var recipe in Resources.LoadAll<CraftRecipe>("CraftingRecipes"))
            {
                if (recipe == null || _recipes.ContainsKey(recipe.name))
                    continue;

                _recipes.Add(recipe.name, recipe);
            }
        }

        public static bool TryCraft(ICrafter crafter, CraftRecipe recipe, int count = 1)
        {
            if (!HasIngredients(crafter, recipe, count))
                return false;

            RemoveIngredients(crafter, recipe, count);
            AddOutputs(crafter, recipe, count);
            return true;
        }

        private static bool HasIngredients(ICrafter crafter, CraftRecipe recipe, int count)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                int total = crafter.Inventory.GetItemNumber(ingredient.item);
                if (total < ingredient.amount * count)
                    return false;
            }

            return true;
        }

        private static void RemoveIngredients(ICrafter crafter, CraftRecipe recipe, int count)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                int totalToRemove = ingredient.amount * count;
                crafter.Inventory.RemoveItem(ingredient.item, totalToRemove);
            }
        }

        private static void AddOutputs(ICrafter crafter, CraftRecipe recipe, int count)
        {
            foreach (var output in recipe.Outputs)
            {
                int totalToAdd = output.amount * count;
                crafter.Inventory.TryAddToFirstAvailableSlot(output.item, totalToAdd);
            }
        }

        public static IEnumerable<CraftRecipe> GetAllRecipes() => _recipes.Values;

        public static CraftRecipe GetRecipe(string name) =>
            _recipes.TryGetValue(name, out var r) ? r : null;
    }
}