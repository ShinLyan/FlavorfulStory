using FlavorfulStory.AI.NonInteractableNpc;
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
            Context?.Set<ShopObject>(ContextType.SelectedObject, furniture);
            Context?.Set(ContextType.AnimationType, furniture.InteractableObjectAnimation);
            Context?.Set(ContextType.AnimationTime, 3f);

            var accessiblePoint = furniture.GetAccessiblePoint();

            _movementController.SetPoint(accessiblePoint); //TODO: добавить поворот в сторону точки
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}