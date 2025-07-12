using FlavorfulStory.Actions;
using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.SceneManagement.ShopLocation;

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
            if (Context != null && Context.TryGet<Shelf>("SelectedShelf", out var shelf)) shelf.SetOccupied(false);
            // var item = shelf.Items[Random.Range(0, shelf.Items.Count)]; //TODO
            // _itemHandler.EquipItem(item);

            var accessiblePoint = _shopLocation.CashDesk.GetAccessiblePoint();
            Context?.Set("CashDeskPoint", accessiblePoint);

            var point = new SchedulePoint(); //TODO: rework
            point.Position = accessiblePoint.position;
            point.LocationName = LocationName.NewShop;
            point.Rotation = accessiblePoint.rotation.eulerAngles;

            _movementController.SetPoint(point);
        }

        public override bool IsComplete() => true;
    }
}