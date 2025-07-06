using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.SceneManagement.ShopLocation;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    public class ShelfPickerState : CharacterState
    {
        private readonly NonInteractableNpcMovementController _movementController;

        private readonly ShopLocation _shopLocation;

        public ShelfPickerState(NonInteractableNpcMovementController movementController, ShopLocation shopLocation)
        {
            _movementController = movementController;
            _shopLocation = shopLocation;
        }

        public override void Enter()
        {
            var availableShelves = _shopLocation.GetAvailableShelves();
            var shelf = availableShelves[Random.Range(0, availableShelves.Length)];
            var point = shelf.GetAccessiblePoint();
            _movementController.SetPoint(point);
        }
    }
}