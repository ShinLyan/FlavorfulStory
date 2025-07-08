using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.SceneManagement.ShopLocation;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    public class RandomPointPickerState : CharacterState
    {
        private readonly NonInteractableNpcMovementController _movementController;

        private readonly ShopLocation _shopLocation;

        public RandomPointPickerState(NonInteractableNpcMovementController movementController,
            ShopLocation shopLocation)
        {
            _movementController = movementController;
            _shopLocation = shopLocation;
        }

        public override void Enter()
        {
            base.Enter();
            var pointVector = _shopLocation.GetRandomPointOnNavMesh(); //TODO: реворк метод

            if (IsPointAvailable(pointVector))
            {
                var point = new SchedulePoint(); //TODO: rework
                point.Position = pointVector;
                point.LocationName = LocationName.NewShop;

                _movementController.SetPoint(point);
            }
            else
            {
                Enter();
            }
        }

        private bool IsPointAvailable(Vector3 point) { return true; } //TODO: реализовать метод

        public override bool IsComplete() => true;
    }
}