using Cysharp.Threading.Tasks;
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
        /// <param name="purchaseIndicator"></param>
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
            if (Context != null && Context.TryGet<ShopObject>(FsmContextType.SelectedObject, out var showcase))
            {
                var showcaseInventory = ((Showcase)showcase).Inventory;
                var itemStack = showcaseInventory.GetRandomStack();
                Context?.Set(FsmContextType.PurchaseItem, itemStack);
                _purchaseIndicator.ShowModel().Forget();
            }

            var accessiblePoint = _shopLocation.CashRegister.GetAccessiblePoint();

            if (accessiblePoint.HasValue)
            {
                Context?.Set(FsmContextType.AnimationType, _shopLocation.CashRegister.InteractableObjectAnimation);
                Context?.Set(FsmContextType.AnimationTime, 3f);
                Context?.Set(FsmContextType.DestinationPoint, accessiblePoint.Value);

                _shopLocation.CashRegister.SetPointOccupancy(accessiblePoint.Value.Position, true);
                _movementController.OnDestinationReached +=
                    () => _shopLocation.CashRegister.SetPointOccupancy(accessiblePoint.Value.Position, false);
            }
            else
            {
                Debug.LogWarning("No accessible point found for the ShowCase!");
            }
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}