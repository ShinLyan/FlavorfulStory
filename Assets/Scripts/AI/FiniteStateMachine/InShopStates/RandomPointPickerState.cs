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
            var pointVector = _shopLocation.GetRandomPointOnNavMesh();

            var point = new SchedulePoint(); //TODO: переделать после удаление WarpGraph
            point.Position = pointVector;
            point.LocationName = LocationName.NewShop;
            point.Rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0).eulerAngles;

            _movementController.SetPoint(point);
        }

        public override bool IsComplete() => true;
    }
}