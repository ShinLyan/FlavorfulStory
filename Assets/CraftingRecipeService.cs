using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Crafting;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory
{
    public class CraftingRecipeService : MonoBehaviour, ICraftingRecipeProvider, ICraftingRecipeUnlocker, ISaveable
    {
        private readonly Dictionary<string, CraftingRecipe> _db = new();
        private readonly HashSet<string> _unlocked = new();
        
        public event Action<CraftingRecipe> RecipeUnlocked;
        private bool _isWarmedUp;
        
        private void Awake()
        {
            if (!_isWarmedUp) WarmUp();
        }
        
        /// <summary> Инициализация базы (вызовите в инсталлере / бутстрапе один раз). </summary>
        /// 
        private void WarmUp()
        {
            _db.Clear();
            foreach (var recipe in Resources.LoadAll<CraftingRecipe>(string.Empty))
            {
                if (string.IsNullOrWhiteSpace(recipe.RecipeID))
                {
                    Debug.LogError($"[Crafting] Recipe '{recipe.name}' has empty RecipeID.");
                    continue;
                }
                if (!_db.TryAdd(recipe.RecipeID, recipe))
                {
                    Debug.LogError($"[Crafting] Duplicate RecipeID '{recipe.RecipeID}' in {recipe}.");
                    continue;
                }

                // Стартовое состояние берём из SO: если IsLocked == false -> по умолчанию открыт.
                if (!recipe.IsLocked) _unlocked.Add(recipe.RecipeID);
            }
        }
        
        #region RecipeProvider
        public IReadOnlyList<CraftingRecipe> All => _db.Values.ToList();

        public CraftingRecipe GetById(string id) => _db.GetValueOrDefault(id);

        public bool IsUnlocked(string id) => _unlocked.Contains(id);
        #endregion

        #region RecipeUnlocker
        public bool Unlock(string id)
        {
            if (!_db.ContainsKey(id)) return false;
            if (!_unlocked.Add(id)) return false;

            RecipeUnlocked?.Invoke(_db[id]);
            return true;
        }

        public bool Unlock(CraftingRecipe recipe) => recipe != null && Unlock(recipe.RecipeID);
        #endregion

        #region Save/Load
        [Serializable]
        private class CraftingSaveData
        {
            public List<string> UnlockedRecipes;
        }

        public object CaptureState() => new CraftingSaveData
        {
            UnlockedRecipes = _unlocked.ToList()
        };

        public void RestoreState(object state)
        {
            _unlocked.Clear();

            // Всегда сначала перезагружаем базу — вдруг сцена загрузилась раньше WarmUp
            if (_db.Count == 0) WarmUp();

            if (state is CraftingSaveData { UnlockedRecipes: not null } data)
            {
                foreach (var id in data.UnlockedRecipes.Where(id => _db.ContainsKey(id))) _unlocked.Add(id);
            }
            else
            {
                foreach (var r in _db.Values.Where(r => !r.IsLocked)) _unlocked.Add(r.RecipeID);
            }
        }
        #endregion
    }
}