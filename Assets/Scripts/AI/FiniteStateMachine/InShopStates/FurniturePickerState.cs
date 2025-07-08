using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.SceneManagement;
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
            base.Enter();
            var furniture = _shopLocation.GetAvailableFurniture();
            Context?.Set("SelectedFurniture", furniture);

            var pointVector = furniture.GetAccessiblePoint();

            var point = new SchedulePoint(); //TODO: rework
            point.Position = pointVector;
            point.LocationName = LocationName.NewShop;

            _movementController.SetPoint(point);
        }

        public override bool IsComplete() => true;
    }
}