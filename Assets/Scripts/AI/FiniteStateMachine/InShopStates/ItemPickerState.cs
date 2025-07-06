using FlavorfulStory.Actions;
using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.SceneManagement.ShopLocation;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    public class ItemPickerState : CharacterState
    {
        private ItemHandler _itemHandler;

        private readonly ShopLocation _shopLocation;

        private NonInteractableNpcMovementController _movementController;

        public ItemPickerState(ItemHandler itemHandler, ShopLocation shopLocation,
            NonInteractableNpcMovementController npcMovementController)
        {
            _itemHandler = itemHandler;
            _shopLocation = shopLocation;
            _movementController = npcMovementController;
        }

        public override void Enter()
        {
            // shelf = Context.TryGet<Shelf>("Shelf") //TODO
            // item = shelf.Items[Random.Range(0, helf.Items.Count)]
            // _itemHandler.EquipItem(item);
            // point = _shopLocation.CashDesk.GetAccessiblePoint();
            // _movementController.SetPoint(point);
        }
    }
}