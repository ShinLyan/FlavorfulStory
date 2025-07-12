using FlavorfulStory.Actions;
using FlavorfulStory.SceneManagement.ShopLocation;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    public class PaymentState : CharacterState
    {
        private readonly ItemHandler _itemHandler;

        private readonly ShopLocation _shopLocation;

        public PaymentState(ShopLocation shopLocation, ItemHandler itemHandler)
        {
            _shopLocation = shopLocation;
            _itemHandler = itemHandler;
        }

        public override void Enter()
        {
            if (Context != null && Context.TryGet<Transform>("CashDeskPoint", out var point))
                _shopLocation.CashDesk.ReleasePoint(point);
            // _itemHandler.EquipItem(item); // TODO
        }

        public override bool IsComplete() => true;
    }
}