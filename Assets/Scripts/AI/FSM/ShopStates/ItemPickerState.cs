using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.Shop;
using UnityEngine;

namespace FlavorfulStory.AI.FSM.ShopStates
{
    /// <summary> Состояние для выбора предмета и перемещения к кассе. </summary>
    public class ItemPickerState : CharacterState
    {
        /// <summary> Локация магазина для получения информации о кассе. </summary>
        private readonly ShopLocation _shopLocation;

        /// <summary> Контроллер движения неинтерактивного NPC. </summary>
        private readonly NpcMovementController _movementController;

        /// <summary> Индикатор купленного товара у НПС. </summary>
        private readonly NpcPurchaseIndicator _purchaseIndicator;

        /// <summary> Инициализирует новый экземпляр состояния выбора предмета. </summary>
        /// <param name="npcMovementController"> Контроллер движения для управления перемещением NPC. </param>
        /// <param name="shopLocation"> Локация магазина для получения информации о кассе. </param>
        /// <param name="purchaseIndicator"> Индикатор покупки NPC. </param>
        public ItemPickerState(NpcMovementController npcMovementController, ShopLocation shopLocation,
            NpcPurchaseIndicator purchaseIndicator)
        {
            _shopLocation = shopLocation;
            _movementController = npcMovementController;
            _purchaseIndicator = purchaseIndicator;
        }

        /// <summary> Выполняет вход в состояние, освобождает полку и устанавливает цель движения к кассе. </summary>
        public override void Enter()
        {
            SelectItem();
            ResolveCashRegisterDestination();
        }

        /// <summary> Выбирает случайный предмет из витрины и сохраняет его в контексте. </summary>
        private void SelectItem()
        {
            if (Context == null || !Context.TryGet<ShopObject>(FsmContextType.SelectedObject, out var shopObject) ||
                shopObject is not Showcase showcase)
                return;

            int slotIndex = showcase.Inventory.GetRandomNonEmptySlotIndex();
            if (slotIndex == -1) return;

            var itemStack = showcase.Inventory.ExtractStackFromSlot(slotIndex);
            Context.Set(FsmContextType.PurchaseItem, itemStack);
            _purchaseIndicator.ShowModel();
        }

        /// <summary> Определяет доступную точку у кассы и сохраняет её в контексте. </summary>
        private void ResolveCashRegisterDestination()
        {
            var destination = _shopLocation.CashRegister.GetAccessiblePoint();
            if (!destination.HasValue)
            {
                Debug.LogWarning("No accessible point found for the ShowCase!");
                return;
            }

            var point = destination.Value;

            Context?.Set(FsmContextType.AnimationType, _shopLocation.CashRegister.InteractableObjectAnimation);
            Context?.Set(FsmContextType.AnimationTime, 3f);
            Context?.Set(FsmContextType.DestinationPoint, point);

            _shopLocation.CashRegister.SetPointOccupancy(point.Position, true);
            _movementController.OnDestinationReached +=
                () => _shopLocation.CashRegister.SetPointOccupancy(point.Position, false);
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда <c>true</c>, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}