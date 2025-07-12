using FlavorfulStory.SceneManagement.ShopLocation;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    public class RefuseItemState : CharacterState
    {
        private readonly ShopLocation _shopLocation;

        public RefuseItemState(ShopLocation shopLocation) { _shopLocation = shopLocation; }

        public override void Enter()
        {
            base.Enter();
            if (Context != null && Context.TryGet<Shelf>("SelectedShelf", out var shelf)) shelf.SetOccupied(false);
        }

        public override bool IsComplete() => true;
    }
}