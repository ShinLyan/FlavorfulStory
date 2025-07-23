using System.Collections.Generic;
using UnityEngine;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.Crafting
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Crafting/Recipe", fileName = "CraftingRecipe")]
    public class CraftingRecipe : ScriptableObject
    {
        /// <summary> Уникальный идентификатор рецепта. </summary>
        [field: Tooltip("ID рецепта."), SerializeField]
        public string RecipeID { get; private set; }

        /// <summary> Указывает, открыт ли рецепт. </summary>
        [field: Tooltip("Открыт ли рецепт?"), SerializeField]
        public bool IsLocked { get; private set; }

        /// <summary> Отображаемое имя рецепта. </summary>
        [field: Tooltip("Отображаемое название рецепта."), SerializeField]
        public string RecipeName { get; private set; }
        
        //TODO: А я говорил, что описание не надо удалять!!!
        /// <summary> Отображаемое имя рецепта. </summary>
        [field: Tooltip("Отображаемое название рецепта."), SerializeField]
        public string Description { get; private set; }

        /// <summary> Длительность крафта в секундах. </summary>
        [field: Tooltip("Длительность крафта."), SerializeField]
        public float Duration { get; private set; }

        /// <summary> Список предметов, необходимых для крафта. </summary>
        [field: Tooltip("Требование рецепта по ресурсам."), SerializeField]
        public List<ItemStack> InputItems { get; private set; }

        /// <summary> Список предметов, получаемых в результате крафта. </summary>
        [field: Tooltip("Результат крафта рецепта."), SerializeField]
        public List<ItemStack> OutputItems { get; private set; }

        /// <summary> Иконка рецепта. </summary>
        [field: Tooltip("Спрайт крафта рецепта."), SerializeField]
        public Sprite Sprite { get; private set; }
    }
}