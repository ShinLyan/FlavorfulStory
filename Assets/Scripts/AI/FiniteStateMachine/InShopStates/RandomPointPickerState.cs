using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.SceneManagement.ShopLocation;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    /// <summary> Состояние для выбора случайной точки на навигационной сетке и перемещения к ней. </summary>
    public class RandomPointPickerState : CharacterState
    {
        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NonInteractableNpcMovementController _movementController;

        /// <summary> Локация магазина для получения случайной точки на навигационной сетке. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Инициализирует новый экземпляр состояния выбора случайной точки. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="shopLocation"> Локация магазина для получения случайной точки. </param>
        public RandomPointPickerState(NonInteractableNpcMovementController movementController,
            ShopLocation shopLocation)
        {
            _movementController = movementController;
            _shopLocation = shopLocation;
        }

        /// <summary> Выполняет вход в состояние, выбирает случайную точку и устанавливает цель движения. </summary>
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

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}