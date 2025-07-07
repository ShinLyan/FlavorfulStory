using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.SceneManagement.ShopLocation;

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
            var availableShelf = _shopLocation.GetAvailableShelf();
            Context?.Set("SelectedShelf", availableShelf);

            var point = availableShelf.GetAccessiblePoint();
            _movementController.SetPoint(point);

            RequestStateChange(GetType());
        }
    }
}