using FlavorfulStory.Shop;

namespace FlavorfulStory.AI.FSM.ShopStates
{
    /// <summary> Состояние освобождения занятого объекта в магазине. </summary>
    public class ReleaseObjectState : CharacterState
    {
        /// <summary> Выполняется при входе в состояние. </summary>
        public override void Enter()
        {
            if (Context.TryGet<ShopObject>(FsmContextType.SelectedObject, out var shopObject))
                shopObject.IsOccupied = false;
        }

        /// <summary> Проверяет завершение состояния. </summary>
        /// <returns> Всегда true, так как состояние выполняется мгновенно </returns>
        public override bool IsComplete() => true;
    }
}