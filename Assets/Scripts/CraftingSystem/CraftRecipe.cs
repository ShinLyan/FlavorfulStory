using System;
using System.Collections.Generic;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.CraftingSystem
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Crafting/Recipe")]
    public class CraftRecipe : ScriptableObject
    {
        [Header("Инфо")]
        public Sprite icon;
        public string displayName;

        [Header("Условия")]
        public CraftingStationType station;
        public float craftTime;

        [Header("Состав")]
        public List<RecipeIngredient> ingredients;
        public List<RecipeOutput> outputs;

        public IReadOnlyList<RecipeIngredient> Ingredients => ingredients;
        public IReadOnlyList<RecipeOutput> Outputs => outputs;
    }

    [Serializable]
    //TODO: Почему это + RecipeOutput. Зачем разные имена.
    public struct RecipeIngredient
    {
        public InventoryItem item;
        public int amount;
    }

    [Serializable]
    public struct RecipeOutput
    {
        public InventoryItem item;
        public int amount;
    }
}
