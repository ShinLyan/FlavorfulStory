using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.SceneManagement.ShopLocation;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    public class FurniturePickerState : CharacterState
    {
        private readonly NonInteractableNpcMovementController _movementController;

        private readonly ShopLocation _shopLocation;

        public FurniturePickerState(NonInteractableNpcMovementController movementController, ShopLocation shopLocation)
        {
            _movementController = movementController;
            _shopLocation = shopLocation;
        }

        public override void Enter()
        {
            var availableFurniture = _shopLocation.GetAvailableFurniture();
            var furniture = availableFurniture[Random.Range(0, availableFurniture.Length)];
            var point = furniture.GetAccessiblePoint();
            _movementController.SetPoint(point);
        }
    }
}