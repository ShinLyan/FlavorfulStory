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

            if (!availableShelf) return;

            availableShelf.SetOccupied(true);
            Context?.Set("SelectedShelf", availableShelf);

            var freePoint = availableShelf.GetAccessiblePoint();

            var point = new SchedulePoint(); //TODO: переделать после удаление WarpGraph
            point.Position = freePoint.position;
            point.LocationName = LocationName.NewShop;
            point.Rotation = freePoint.rotation.eulerAngles;

            _movementController.SetPoint(point);
        }

        public override bool IsComplete() => true;
    }
}