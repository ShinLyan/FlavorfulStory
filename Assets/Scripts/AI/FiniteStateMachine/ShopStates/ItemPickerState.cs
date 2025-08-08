using System.Collections.Generic;
using FlavorfulStory.AI.NonInteractableNpc;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Shop;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    /// <summary> Состояние для выбора предмета и перемещения к кассе. </summary>
    public class ItemPickerState : CharacterState
    {
        /// <summary> Локация магазина для получения информации о кассе. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NonInteractableNpcMovementController _movementController;

        /// <summary> Инициализирует новый экземпляр состояния выбора предмета. </summary>
        /// <param name="npcMovementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="shopLocation"> Локация магазина для получения информации о кассе. </param>
        public ItemPickerState(NonInteractableNpcMovementController npcMovementController, ShopLocation shopLocation)
        {
            _shopLocation = shopLocation;
            _movementController = npcMovementController;
        }

        /// <summary> Выполняет вход в состояние, освобождает полку и устанавливает цель движения к кассе. </summary>
        public override void Enter()
        {
            base.Enter();

            if (Context != null && Context.TryGet<ShopObject>(ContextType.SelectedObject, out var showcase))
            {
                var showcaseInventory = showcase.gameObject.GetComponent<Inventory>();
                var itemStack = GetRandomStackFromInventory(showcaseInventory);
                Context?.Set(ContextType.PurchaseItem, itemStack);
            }

            var accessiblePoint = _shopLocation.CashRegister.GetAccessiblePoint();
            _shopLocation.CashRegister.SetPointOccupancy(accessiblePoint, true);

            Context?.Set(ContextType.CashDeskPoint, accessiblePoint);
            Context?.Set(ContextType.AnimationType, _shopLocation.CashRegister.InteractableObjectAnimation);
            Context?.Set(ContextType.AnimationTime, 3f);

            _movementController.SetPoint(accessiblePoint.position); //TODO: добавить поворот в сторону точки
            _movementController.OnDestinationReached +=
                () => _shopLocation.CashRegister.SetPointOccupancy(accessiblePoint, false);
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;

        private static ItemStack GetRandomStackFromInventory(Inventory inventory)
        {
            var nonEmptySlots = new List<int>();

            for (int i = 0; i < inventory.InventorySize; i++)
            {
                var stackSlot = inventory.GetItemStackInSlot(i);
                if (stackSlot.Item != null && stackSlot.Number > 0) nonEmptySlots.Add(i);
            }

            int randomIndex = Random.Range(0, nonEmptySlots.Count);
            var stack = inventory.GetItemStackInSlot(nonEmptySlots[randomIndex]);
            inventory.RemoveFromSlot(randomIndex);
            return stack;
        }
    }
}