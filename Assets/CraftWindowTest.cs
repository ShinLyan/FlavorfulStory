using System;
using UnityEngine;

namespace FlavorfulStory
{
    public class CraftWindowTest : MonoBehaviour
    {
        [SerializeField] private CraftingWindow _craftingWindow;

        private void Start()
        {
            _craftingWindow.Setup(CraftingRecipeProvider.GetAllRecipes());
        }
    }
}