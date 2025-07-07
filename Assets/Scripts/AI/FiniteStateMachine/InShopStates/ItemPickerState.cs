using FlavorfulStory.Actions;
using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.SceneManagement.ShopLocation;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    public class ItemPickerState : CharacterState
    {
        private readonly ItemHandler _itemHandler;

        private readonly ShopLocation _shopLocation;

        private readonly NonInteractableNpcMovementController _movementController;

        public ItemPickerState(NonInteractableNpcMovementController npcMovementController, ShopLocation shopLocation,
            ItemHandler itemHandler)
        {
            _shopLocation = shopLocation;
            _movementController = npcMovementController;
            _itemHandler = itemHandler;
        }

        public override void Enter()
        {
            if (Context != null && Context.TryGet<Shelf>("SelectedShelf", out var shelf))
            {
                // var item = shelf.Items[Random.Range(0, shelf.Items.Count)]; //TODO
                // _itemHandler.EquipItem(item);

                var point = _shopLocation.CashDesk.GetAccessiblePoint();
                _movementController.SetPoint(point);
                RequestStateChange(GetType());
            }
            else
            {
                Debug.LogError("Shelf not found in context!");
                RequestStateChange(GetType());
            }

            // shelf = Context.TryGet<Shelf>("Shelf") //TODO
            // item = shelf.Items[Random.Range(0, helf.Items.Count)]
            // _itemHandler.EquipItem(item);
            // point = _shopLocation.CashDesk.GetAccessiblePoint();
            // _movementController.SetPoint(point);
        }
    }
}