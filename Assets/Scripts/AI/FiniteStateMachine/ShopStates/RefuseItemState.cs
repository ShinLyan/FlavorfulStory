using FlavorfulStory.Shop;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    /// <summary> Состояние для отказа от выбранного предмета и освобождения полки. </summary>
    public class RefuseItemState : CharacterState
    {
        /// <summary> Локация магазина для взаимодействия с полками. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Инициализирует новый экземпляр состояния отказа от предмета. </summary>
        /// <param name="shopLocation"> Локация магазина для доступа к полкам. </param>
        public RefuseItemState(ShopLocation shopLocation) { _shopLocation = shopLocation; }

        /// <summary> Выполняет вход в состояние и освобождает выбранную полку. </summary>
        public override void Enter()
        {
            base.Enter();
            if (Context != null && Context.TryGet<ShopObject>(ContextType.SelectedObject, out var shelf))
                shelf.IsOccupied = false;
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}