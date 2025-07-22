using FlavorfulStory.Actions;
using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.Economy;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    /// <summary> Состояние для выбора предмета и перемещения к кассе. </summary>
    public class ItemPickerState : CharacterState
    {
        /// <summary> Обработчик предметов для экипировки выбранных товаров. </summary>
        private readonly ItemHandler _itemHandler;

        /// <summary> Локация магазина для получения информации о кассе. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NonInteractableNpcMovementController _movementController;

        private TransactionService _transactionService;

        /// <summary> Инициализирует новый экземпляр состояния выбора предмета. </summary>
        /// <param name="npcMovementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="shopLocation"> Локация магазина для получения информации о кассе. </param>
        /// <param name="itemHandler"> Обработчик предметов для экипировки товаров. </param>
        /// <param name="transactionService"> </param>
        public ItemPickerState(NonInteractableNpcMovementController npcMovementController, ShopLocation shopLocation,
            ItemHandler itemHandler)
        {
            _shopLocation = shopLocation;
            _movementController = npcMovementController;
            _itemHandler = itemHandler;
        }

        /// <summary> Выполняет вход в состояние, освобождает полку и устанавливает цель движения к кассе. </summary>
        public override void Enter()
        {
            base.Enter();

            Debug.Log(_itemHandler);
            if (Context != null && Context.TryGet<Shelf>("SelectedShelf", out var shelf)) shelf.IsOccupied = false;
            // var item = shelf.Items[Random.Range(0, shelf.Items.Count)]; //TODO

            //TODO: тестовый предмет, переделать на предмет с полки
            var item = ItemDatabase.GetItemFromID("460b1671-464a-4d0f-94a8-fe034fcb7ea2");
            bool itemPrefab = item.PickupPrefab.GetComponent<Rigidbody>().useGravity = false;
            var itemStack = new ItemStack { Item = item, Number = 1 };
            _itemHandler.EquipItem(itemStack);

            var accessiblePoint = _shopLocation.CashDesk.GetAccessiblePoint();
            Context?.Set("CashDeskPoint", accessiblePoint);
            Context?.Set("PurchaseItem", itemStack);


            var point = new SchedulePoint(); //TODO: rework
            point.Position = accessiblePoint.position;
            point.LocationName = LocationName.NewShop;
            point.Rotation = accessiblePoint.rotation.eulerAngles;

            _movementController.SetPoint(point);
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}