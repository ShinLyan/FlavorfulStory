using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.SceneManagement;
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
            base.Enter();
            var availableShelf = _shopLocation.GetAvailableShelf();
            availableShelf.SetOccupied(true);
            Context?.Set("SelectedShelf", availableShelf);

            var pointVector = availableShelf.GetAccessiblePoint();

            var point = new SchedulePoint(); //TODO: rework
            point.Position = pointVector;
            point.LocationName = LocationName.NewShop;

            _movementController.SetPoint(point);
        }

        public override bool IsComplete() => true;
    }
}