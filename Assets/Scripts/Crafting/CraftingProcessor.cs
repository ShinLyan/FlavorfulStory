using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.Crafting
{
    /// <summary> Класс, отвечающий за выполнение рецепта, валидацию и распределение предметов. </summary>
    public static class CraftingProcessor
    {
        /// <summary> Выполняет рецепт крафта, проверяя условия и добавляя предметы в инвентарь. </summary>
        /// <param name="recipe"> Рецепт крафта. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="onComplete"> Действие по завершении крафта. </param>
        public static async UniTask ExecuteRecipe(CraftingRecipe recipe, int count, Inventory inventory,
            Action onComplete)
        {
            if (CanCraft(recipe, count, inventory) != CraftingResult.Success)
            {
                Debug.LogWarning($"{nameof(CraftingProcessor)}: Attempted to craft with invalid conditions.");
                return;
            }

            RemoveRequiredItems(recipe, count, inventory);

            float duration = recipe.Duration * count;
            if (duration > 0) await UniTask.Delay(TimeSpan.FromSeconds(duration));

            AddCraftedItemsToInventory(recipe, count, inventory);

            onComplete?.Invoke();
        }

        /// <summary> Проверяет, возможно ли выполнение рецепта. </summary>
        /// <param name="recipe"> Рецепт крафта. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <returns> <see cref="CraftingResult"/>: результат проверки крафта. </returns>
        public static CraftingResult CanCraft(CraftingRecipe recipe, int count, Inventory inventory)
        {
            if (!PlayerHasEnoughItems(recipe, count, inventory)) return CraftingResult.NotEnoughResources;

            if (!PlayerHasSpaceForCraftedItems(recipe, count, inventory)) return CraftingResult.NotEnoughSpace;

            return CraftingResult.Success;
        }

        /// <summary> Удаляет требуемые предметы из инвентаря. </summary>
        /// <param name="recipe"> Рецепт крафта. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        private static void RemoveRequiredItems(CraftingRecipe recipe, int count, Inventory inventory)
        {
            foreach (var input in recipe.InputItems)
            {
                int totalAmount = input.Number * count;
                inventory.RemoveItem(input.Item, totalAmount);
            }
        }

        /// <summary> Добавляет готовые предметы в инвентарь или логирует предупреждение при нехватке места. </summary>
        /// <param name="recipe"> Рецепт крафта. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        private static void AddCraftedItemsToInventory(CraftingRecipe recipe, int count, Inventory inventory)
        {
            foreach (var output in recipe.OutputItems)
            {
                int totalAmount = output.Number * count;
                if (!inventory.TryAddToFirstAvailableSlot(output.Item, totalAmount))
                    // TODO: IItemDropService.Drop(_dropPoint, output.Item, totalAmount);
                    Debug.LogWarning($"Not enough space for crafted item: {output.Item.ItemName}, {totalAmount}");
            }
        }

        /// <summary> Проверяет, есть ли в инвентаре все необходимые ресурсы. </summary>
        /// <param name="recipe"> Рецепт крафта. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <returns> True, если ресурсов достаточно. </returns>
        private static bool PlayerHasEnoughItems(CraftingRecipe recipe, int count, Inventory inventory)
        {
            foreach (var itemRequirement in recipe.InputItems)
            {
                int requiredAmount = itemRequirement.Number * count;
                if (inventory.GetItemNumber(itemRequirement.Item) < requiredAmount) return false;
            }

            return true;
        }

        /// <summary> Проверяет, есть ли в инвентаре место для результатов крафта. </summary>
        /// <param name="recipe"> Рецепт крафта. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <returns> True, если места достаточно. </returns>
        private static bool PlayerHasSpaceForCraftedItems(CraftingRecipe recipe, int count, Inventory inventory)
        {
            //TODO: Тут возможно стоит сперва вычесть из инвентаря требования крафта и только потом добавлять
            // Пока оставляю так - тк игрок не "помещает" предметы в крафт
            foreach (var output in recipe.OutputItems)
            {
                int totalAmount = output.Number * count;
                if (!inventory.HasSpaceFor(output.Item, totalAmount)) return false;
            }

            return true;
        }
    }
}