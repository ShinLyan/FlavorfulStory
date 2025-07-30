using FlavorfulStory.Economy;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Shop;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    /// <summary> Состояние для обработки оплаты товаров у кассы. </summary>
    public class PaymentState : CharacterState
    {
        /// <summary> Менеджер локаций. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Сервис для обработки транзакций и торговых операций. </summary>
        private readonly TransactionService _transactionService;

        /// <summary> Инициализирует новый экземпляр состояния оплаты. </summary>
        /// <param name="locationManager"> Локация магазина для доступа к кассе. </param>\
        /// <param name="transactionService"> Сервис для обработки транзакций. </param>
        public PaymentState(LocationManager locationManager, TransactionService transactionService)
        {
            _locationManager = locationManager;
            _transactionService = transactionService;
        }

        /// <summary> Выполняет вход в состояние, освобождает точку у кассы и обрабатывает оплату. </summary>
        public override void Enter()
        {
            base.Enter();

            if (Context != null && Context.TryGet<Transform>(ContextType.CashDeskPoint, out var point))
                ((ShopLocation)_locationManager.GetLocationByName(LocationName.NewShop)).CashDesk.ReleasePoint(point);

            if (Context != null && Context.TryGet<ItemStack>(ContextType.PurchaseItem, out var itemStack))
            {
                bool playerInLocation = _locationManager.IsPlayerInLocation(LocationName.NewShop);
                _transactionService.SellToNpc(itemStack, playerInLocation);
            }
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}