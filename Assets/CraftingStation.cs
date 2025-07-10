using System;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Actions;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    public class CraftingStation : MonoBehaviour, IInteractable
    {
        [Inject] private readonly CraftingWindow _craftingWindow;
        [Inject] private readonly Inventory _playerInventory;

        private bool _isCrafting;

        [field: SerializeField] public ActionDescription ActionDescription { get; private set; }
        public bool IsInteractionAllowed => !_isCrafting;

        public float GetDistanceTo(Transform otherTransform)
            => Vector3.Distance(otherTransform.position, transform.position);

        public void BeginInteraction(PlayerController player)
        {
            _craftingWindow.Setup(CraftingRecipeProvider.GetAllRecipes(), 
                () => EndInteraction(player),
                this);
            _craftingWindow.Open();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        public void EndInteraction(PlayerController player)
        {
            player.SetBusyState(false);
            _craftingWindow.Close();
        }
        
        public async void StartCrafting(CraftingRecipe recipe, int count, Action onComplete)
        {
            if (_isCrafting) return;
            _isCrafting = true;
            
            foreach (var input in recipe.InputItems)
            {
                int totalAmount = input.Quantity * count;
                _playerInventory.RemoveItem(input.Item, totalAmount);
            }
            
            float duration = recipe.Duration * count;
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            
            foreach (var output in recipe.OutputItems)
            {
                if (!_playerInventory.TryAddToFirstAvailableSlot(output.Item, output.Quantity * count))
                {
                    //TODO: Govno - подумать
                    Debug.LogWarning($"Not enough space for crafted item: {output.Item.ItemName}");
                }
            }

            _isCrafting = false;
            onComplete?.Invoke();
        }
    }
}