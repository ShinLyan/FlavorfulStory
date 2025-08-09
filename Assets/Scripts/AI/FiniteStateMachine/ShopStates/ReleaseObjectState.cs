using FlavorfulStory.Shop;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    public class ReleaseObjectState : CharacterState
    {
        private readonly ShopLocation _shopLocation;
        public ReleaseObjectState(ShopLocation shopLocation) => _shopLocation = shopLocation;

        /// <summary> Выполняет вход в состояние. </summary>
        public override void Enter()
        {
            base.Enter();
            if (Context == null) return;

            if (Context.TryGet<ShopObject>(ContextType.SelectedObject, out var shopObject))
                shopObject.IsOccupied = false;

            // if (Context.TryGet<Transform>(ContextType.CashDeskPoint, out var point))
            //     _shopLocation.CashRegister.SetPointOccupancy(point, false);
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}