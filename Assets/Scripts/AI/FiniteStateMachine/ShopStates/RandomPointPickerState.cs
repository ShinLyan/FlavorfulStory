using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.Shop;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
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
            var point = _shopLocation.GetRandomPointOnNavMesh();

            Context?.Set(ContextType.AnimationType, AnimationType.Special1); //TODO: добавить анимацию обдумывания
            Context?.Set(ContextType.AnimationTime, 3f);

            _movementController.SetPoint(point); //TODO: добавить поворот в сторону точки
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}