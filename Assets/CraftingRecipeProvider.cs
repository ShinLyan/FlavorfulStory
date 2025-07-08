using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory
{
    //TODO: Дубляж логики с ItemDatabase.cs. Обсудить с Вовой создание generiс варианта или в целом лукап-таблицы
    public static class CraftingRecipeProvider
    {
        /// <summary> Словарь рецептов. </summary>
        /// <remarks> Хранит пары (GUID, предмет). </remarks>
        private static readonly Dictionary<string, CraftingRecipe> _recipeDatabase;

        /// <summary> Инициализирует базу данных предметов. </summary>
        /// <remarks> Загружает все предметы из папки Resources. </remarks>
        static CraftingRecipeProvider()
        {
            _recipeDatabase = new();
            foreach (var recipe in Resources.LoadAll<CraftingRecipe>(string.Empty))
            {
                if (_recipeDatabase.TryGetValue(recipe.RecipeID, out var value))
                {
                    Debug.LogError("Присутствует дубликат ID InventoryItem для объектов: " +
                                   $"{value} и {recipe}. Замените ID у данного объекта.");
                    continue;
                }

                _recipeDatabase[recipe.RecipeID] = recipe;
            }
        }

        /// <summary> Получить все крафт-рецепты. </summary>
        /// <returns> Экземпляр <see cref="InventoryItem"/>, соответствующий ID. </returns>
        public static CraftingRecipe[] GetAllRecipes() => _recipeDatabase.Values.ToArray();

        /// <summary> Получить экземпляр предмета инвентаря по ID. </summary>
        /// <param name="recipeID"> ID предмета инвентаря. </param>
        /// <returns> Экземпляр <see cref="InventoryItem"/>, соответствующий ID. </returns>
        public static CraftingRecipe GetItemFromID(string recipeID)
        {
            if (recipeID == null || !_recipeDatabase.TryGetValue(recipeID, out var recipe)) return null;
            return recipe;
        }
    }
}