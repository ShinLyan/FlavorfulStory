using System;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.Crafting
{
    /// <summary> Класс, отвечающий за выполнение рецепта, валидацию и распределение предметов. </summary>
    public static class CraftingProcessor
    {
        //TODO: комм
        private static ICraftingRecipeProvider _provider;
        private static IInventoryProvider _inventoryProvider;
        
        public static void Init(ICraftingRecipeProvider provider, IInventoryProvider inventoryProvider)
        {
            _provider = provider;
            _inventoryProvider = inventoryProvider;
        }
        
        /// <summary> Выполняет рецепт крафта, проверяя условия и добавляя предметы в инвентарь. </summary>
        /// <param name="recipe"> Рецепт крафта. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="onComplete"> Действие по завершении крафта. </param>
        public static async UniTask ExecuteRecipe(CraftingRecipe recipe, int count,
                                                  Inventory playerInventory, Action onComplete)
        {
            if (CanCraft(recipe, count, playerInventory) != CraftingResult.Success)
            {
                Debug.LogWarning($"{nameof(CraftingProcessor)}: Attempted to craft with invalid conditions.");
                return;
            }

            // ⬇ Сначала списываем из игрока, остаток — из сундуков
            RemoveRequiredItemsPlayerThenChests(recipe, count, playerInventory);

            float duration = recipe.Duration * count;
            if (duration > 0) await UniTask.Delay(TimeSpan.FromSeconds(duration));

            AddCraftedItemsToInventory(recipe, count, playerInventory);
            onComplete?.Invoke();
        }

        /// <summary> Проверяет, возможно ли выполнение рецепта. </summary>
        /// <param name="recipe"> Рецепт крафта. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <returns> <see cref="CraftingResult"/>: результат проверки крафта. </returns>
        public static CraftingResult CanCraft(CraftingRecipe recipe, int count, Inventory playerInventory)
        {if (_provider != null && !_provider.IsUnlocked(recipe.RecipeID))
                return CraftingResult.Locked;

            // ⬇ Ресурсы считаем суммарно: игрок + все сундуки
            if (!PlayerPlusChestsHaveEnoughItems(recipe, count, playerInventory))
                return CraftingResult.NotEnoughResources;

            // ⬇ Места для результата — в рюкзаке игрока (как раньше)
            if (!PlayerHasSpaceForCraftedItems(recipe, count, playerInventory))
                return CraftingResult.NotEnoughSpace;

            return CraftingResult.Success;
        }

        private static bool PlayerPlusChestsHaveEnoughItems(CraftingRecipe recipe, int count, Inventory playerInventory)
        {
            if (_inventoryProvider == null)
            {
                Debug.LogError("[Crafting] InventoryProvider is not initialized.");
                return false;
            }

            var chests = _inventoryProvider.GetAll(InventoryType.Chest);

            foreach (var req in recipe.InputItems)
            {
                int need = req.Number * count;
                int havePlayer = playerInventory.GetItemNumber(req.Item);
                int haveChests = chests.Sum(inv => inv.GetItemNumber(req.Item));
                if (havePlayer + haveChests < need) return false;
            }
            return true;
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

        private static void RemoveRequiredItemsPlayerThenChests(CraftingRecipe recipe, int count, Inventory playerInventory)
        {
            var chests = _inventoryProvider.GetAll(InventoryType.Chest).ToList();

            foreach (var req in recipe.InputItems)
            {
                int remaining = req.Number * count;

                // 1) сначала игрок
                int havePlayer = playerInventory.GetItemNumber(req.Item);
                if (havePlayer > 0)
                {
                    int takeFromPlayer = Math.Min(havePlayer, remaining);
                    playerInventory.RemoveItem(req.Item, takeFromPlayer);
                    remaining -= takeFromPlayer;
                }

                // 2) потом сундуки
                foreach (var chest in chests)
                {
                    if (remaining <= 0) break;

                    int have = chest.GetItemNumber(req.Item);
                    if (have <= 0) continue;

                    int toRemove = Math.Min(have, remaining);
                    chest.RemoveItem(req.Item, toRemove);
                    remaining -= toRemove;
                }

                if (remaining > 0)
                {
                    // Теоретически сюда не попадём, CanCraft уже проверил.
                    Debug.LogWarning($"[Crafting] Missing '{req.Item.ItemName}' during removal: {remaining}");
                }
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
                int total = output.Number * count;
                if (!inventory.TryAddToFirstAvailableSlot(output.Item, total))
                    // TODO: IItemDropService.Drop(_dropPoint, output.Item, totalAmount);
                    Debug.LogWarning($"Not enough space for crafted item: {output.Item.ItemName}, {total}");
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
                int total = output.Number * count;
                if (!inventory.HasSpaceFor(output.Item, total)) return false;
            }
            return true;
        }
    }
}