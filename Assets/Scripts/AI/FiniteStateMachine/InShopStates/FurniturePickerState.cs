using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.SceneManagement.ShopLocation;

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
            var furniture = _shopLocation.GetAvailableFurniture();
            Context?.Set("SelectedFurniture", furniture);

            var point = furniture.GetAccessiblePoint();
            _movementController.SetPoint(point);
        }
    }
}