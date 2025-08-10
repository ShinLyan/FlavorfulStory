using System;
using FlavorfulStory.Crafting;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    public class RecipeUnlocker : MonoBehaviour
    {
        [Inject] private ICraftingRecipeUnlocker _recipeUnlocker;
        
        [SerializeField] private CraftingRecipe _recipe;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _recipeUnlocker.Unlock(_recipe);
            }
        }
    }
}
