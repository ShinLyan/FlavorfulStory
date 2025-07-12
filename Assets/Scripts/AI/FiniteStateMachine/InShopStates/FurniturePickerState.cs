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

            if (!furniture) return;

            furniture.SetOccupied(true);
            Context?.Set("SelectedFurniture", furniture);

            var accessiblePoint = furniture.GetAccessiblePoint();

            var point = new SchedulePoint(); //TODO: rework
            point.Position = accessiblePoint.position;
            point.LocationName = LocationName.NewShop;
            point.Rotation = accessiblePoint.rotation.eulerAngles;

            _movementController.SetPoint(point);
            _movementController.OnDestinationReached += () => furniture.SetOccupied(false);
        }

        public override bool IsComplete() => true;
    }
}