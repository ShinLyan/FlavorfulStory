using System.Collections.Generic;
using FlavorfulStory.BuildingRepair;
using UnityEngine;

namespace FlavorfulStory
{
    //TODO: Сделать isUnlocked вместе с сохранением
    [CreateAssetMenu(menuName = "FlavorfulStory/Crafting/Recipe", fileName = "CraftingRecipe")]
    public class CraftingRecipe : ScriptableObject
    {
        [field: Tooltip("ID рецепта."), SerializeField]
        public string RecipeID { get; private set; }
        [field: Tooltip("Открыт ли рецепт."), SerializeField]
        public bool Locked { get; private set; }
        [field: Tooltip("Отображаемое название рецепта."), SerializeField]
        public string RecipeName { get; private set; }
        [field: Tooltip("Отображаемое описание рецепта."), SerializeField]
        public string RecipeDescription { get; private set; }
        [field: Tooltip("Длительность крафта."), SerializeField]
        public float Duration { get; private set; }
        [field: Tooltip("Требование рецепта по ресурсам."), SerializeField]
        public List<ItemRequirement> InputItems { get; private set; }
        [field: Tooltip("Результат крафта рецепта."), SerializeField]
        public List<ItemRequirement> OutputItems { get; private set; }
        [field: Tooltip("Спрайт крафта рецепта."), SerializeField]
        public Sprite Sprite { get; private set; }
    }
}