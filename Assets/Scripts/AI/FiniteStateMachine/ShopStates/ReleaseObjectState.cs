using FlavorfulStory.Shop;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    public class ReleaseObjectState : CharacterState
    {
        private readonly ShopLocation _shopLocation;
        public ReleaseObjectState(ShopLocation shopLocation) => _shopLocation = shopLocation;

        /// <summary> Выполняет вход в состояние и освобождает выбранную полку. </summary>
        public override void Enter()
        {
            base.Enter();
            if (Context != null && Context.TryGet<ShopObject>(ContextType.SelectedObject, out var shopObject))
                shopObject.IsOccupied = false;

            if (Context != null && Context.TryGet<Transform>(ContextType.CashDeskPoint, out var point))
                _shopLocation.CashDesk.ReleasePoint(point);
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}