using System.Collections.Generic;
using FlavorfulStory.Crafting;
using FlavorfulStory.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory
{
    public class RecipeIOStrip : MonoBehaviour
    {
        [Header("Parents")]
        [SerializeField] private RectTransform _inputsParent;
        [SerializeField] private RectTransform _outputsParent;

        [Header("Prefabs")]
        [SerializeField] private Image _itemIconPrefab;

        private readonly List<GameObject> _spawned = new();

        private void Clear()
        {
            foreach (var t in _spawned)
                if (t) Destroy(t);

            _spawned.Clear();
        }

        public void Build(CraftingRecipe recipe)
        {
            Clear();
            SpawnIcons(_inputsParent, recipe.InputItems);
            SpawnIcons(_outputsParent, recipe.OutputItems);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_inputsParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_outputsParent);
        }

        private void SpawnIcons(Transform parent, IEnumerable<ItemStack> stacks)
        {
            foreach (var stack in stacks)
            {
                var img = Instantiate(_itemIconPrefab, parent);
                img.sprite = stack.Item.Icon;
                img.enabled = img.sprite != null;
                _spawned.Add(img.gameObject);
            }
        }
    }
}