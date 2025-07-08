using FlavorfulStory.Actions;
using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.SceneManagement;
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
            base.Enter();
            if (Context != null && Context.TryGet<Shelf>("SelectedShelf", out var shelf))
            {
                // var item = shelf.Items[Random.Range(0, shelf.Items.Count)]; //TODO
                // _itemHandler.EquipItem(item);
            }
            else
            {
                Debug.LogError("Shelf not found in context!");
            }

            var pointVector = _shopLocation.CashDesk.GetAccessiblePoint();

            var point = new SchedulePoint(); //TODO: rework
            point.Position = pointVector;
            point.LocationName = LocationName.NewShop;

            _movementController.SetPoint(point);
        }

        public override bool IsComplete() => true;
    }
}