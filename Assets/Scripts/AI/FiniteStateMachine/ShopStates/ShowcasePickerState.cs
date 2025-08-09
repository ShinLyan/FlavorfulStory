using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.Shop;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    /// <summary> Состояние для выбора доступной полки и перемещения к ней. </summary>
    public class ShowcasePickerState : CharacterState
    {
        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NonInteractableNpcMovementController _movementController;

        /// <summary> Локация магазина для получения доступных полок. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Инициализирует новый экземпляр состояния выбора полки. </summary>
        /// <param name="movementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="shopLocation"> Локация магазина для доступа к полкам. </param>
        public ShowcasePickerState(NonInteractableNpcMovementController movementController, ShopLocation shopLocation)
        {
            _movementController = movementController;
            _shopLocation = shopLocation;
        }

        /// <summary> Выполняет вход в состояние, выбирает доступную полку и устанавливает цель движения. </summary>
        public override void Enter()
        {
            base.Enter();
            var availableShowcase = _shopLocation.GetRandomAvailableShowcaseWithItems();

            if (!availableShowcase) return;

            availableShowcase.IsOccupied = true;
            Context?.Set<ShopObject>(ContextType.SelectedObject, availableShowcase);
            Context?.Set(ContextType.AnimationType, availableShowcase.InteractableObjectAnimation);
            Context?.Set(ContextType.AnimationTime, 3f);

            var point = availableShowcase.GetAccessiblePoint();

            _movementController.SetPoint(point); //TODO: добавить поворот в сторону точки
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}