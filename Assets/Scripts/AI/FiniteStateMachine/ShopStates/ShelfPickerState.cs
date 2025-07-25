using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    /// <summary> Состояние для выбора доступной полки и перемещения к ней. </summary>
    public class ShelfPickerState : CharacterState
    {
        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NonInteractableNpcMovementController _movementController;

        /// <summary> Локация магазина для получения доступных полок. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Инициализирует новый экземпляр состояния выбора полки. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="shopLocation"> Локация магазина для доступа к полкам. </param>
        public ShelfPickerState(NonInteractableNpcMovementController movementController, ShopLocation shopLocation)
        {
            _movementController = movementController;
            _shopLocation = shopLocation;
        }

        /// <summary> Выполняет вход в состояние, выбирает доступную полку и устанавливает цель движения. </summary>
        public override void Enter()
        {
            base.Enter();
            var availableShelf = _shopLocation.GetAvailableShelf();

            if (!availableShelf) return;

            availableShelf.IsOccupied = true;
            Context?.Set<ShopObject>(ContextType.SelectedObject, availableShelf);
            Context?.Set(ContextType.AnimationType, availableShelf.InteractableObjectAnimation);
            Context?.Set(ContextType.AnimationTime, 3f);

            var freePoint = availableShelf.GetAccessiblePoint();

            var point = new SchedulePoint(); //TODO: переделать после удаление WarpGraph
            point.Position = freePoint.position;
            point.LocationName = LocationName.NewShop;
            point.Rotation = freePoint.rotation.eulerAngles;

            _movementController.SetPoint(point);
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}