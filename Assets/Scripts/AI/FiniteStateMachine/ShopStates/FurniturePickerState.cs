using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    /// <summary> Состояние для выбора мебели и перемещения к ней персонажа. </summary>
    public class FurniturePickerState : CharacterState
    {
        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NonInteractableNpcMovementController _movementController;

        /// <summary> Локация магазина для получения доступной мебели. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Инициализирует новый экземпляр состояния выбора мебели. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="shopLocation"> Локация магазина для поиска доступной мебели. </param>
        public FurniturePickerState(NonInteractableNpcMovementController movementController, ShopLocation shopLocation)
        {
            _movementController = movementController;
            _shopLocation = shopLocation;
        }

        /// <summary> Выполняет вход в состояние, выбирает доступную мебель и устанавливает цель движения. </summary>
        public override void Enter()
        {
            base.Enter();
            var furniture = _shopLocation.GetAvailableFurniture();

            if (!furniture) return;

            furniture.IsOccupied = true;
            Context?.Set("SelectedFurniture", furniture);

            var accessiblePoint = furniture.GetAccessiblePoint();

            var point = new SchedulePoint(); //TODO: rework
            point.Position = accessiblePoint.position;
            point.LocationName = LocationName.NewShop;
            point.Rotation = accessiblePoint.rotation.eulerAngles;

            _movementController.SetPoint(point);
            _movementController.OnDestinationReached += () => furniture.IsOccupied = false;
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}