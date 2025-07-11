using UnityEngine;
using Zenject;
using FlavorfulStory.Actions;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;

namespace FlavorfulStory.Crafting
{
    /// <summary> Станция крафта, с которой может взаимодействовать игрок. </summary>
    public class CraftingStation : MonoBehaviour, IInteractable
    {
        /// <summary> Окно крафта, отображаемое при взаимодействии. </summary>
        [Inject] private readonly CraftingWindow _craftingWindow;
        /// <summary> Инвентарь игрока, участвующий в крафте. </summary>
        [Inject] private readonly Inventory _playerInventory;
        
        [field: SerializeField] public ActionDescription ActionDescription { get; private set; }
        public bool IsInteractionAllowed { get; private set; } = true;
        
        
        public float GetDistanceTo(Transform otherTransform)
            => Vector3.Distance(otherTransform.position, transform.position);

        public void BeginInteraction(PlayerController player)
        {
            _craftingWindow.Setup(
                CraftingRecipeProvider.GetAllRecipes(), 
                OnCraftRequested,
                () => EndInteraction(player)
                );
            
            _craftingWindow.Open();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        public void EndInteraction(PlayerController player)
        {
            player.SetBusyState(false);
        }
        
        /// <summary> Обрабатывает событие запроса крафта. </summary>
        /// <param name="recipe"> Выбранный рецепт. </param>
        /// <param name="count"> Количество создаваемых наборов. </param>
        private async void OnCraftRequested(CraftingRecipe recipe, int count)
        {
            if (!IsInteractionAllowed)
            {
                Debug.LogWarning("Crafting is already in progress.");
                return;
            }

            var result = CraftingProcessor.CanCraft(recipe, count, _playerInventory);
            if (result != CraftingResult.Success)
            {
                Debug.LogWarning($"Crafting aborted: {result}");
                return;
            }

            IsInteractionAllowed = false;

            await CraftingProcessor.ExecuteRecipe(recipe, count, _playerInventory, () =>
            {
                IsInteractionAllowed = true;
                _craftingWindow.OnCraftCompleted();
            });
        }
    }
}