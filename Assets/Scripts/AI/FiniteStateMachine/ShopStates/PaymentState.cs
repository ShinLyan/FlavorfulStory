using FlavorfulStory.Economy;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.SceneManagement;

namespace FlavorfulStory.AI.FiniteStateMachine.ShopStates
{
    /// <summary> Состояние для обработки оплаты товаров у кассы. </summary>
    public class PaymentState : CharacterState
    {
        /// <summary> Менеджер локаций. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Сервис для обработки транзакций и торговых операций. </summary>
        private readonly TransactionService _transactionService;

        private readonly NpcSpriteIndicator _spriteIndicator;

        /// <summary> Инициализирует новый экземпляр состояния оплаты. </summary>
        /// <param name="locationManager"> Локация магазина для доступа к кассе. </param>\
        /// <param name="transactionService"> Сервис для обработки транзакций. </param>
        public PaymentState(LocationManager locationManager, TransactionService transactionService,
            NpcSpriteIndicator spriteIndicator)
        {
            _locationManager = locationManager;
            _transactionService = transactionService;
            _spriteIndicator = spriteIndicator;
        }

        /// <summary> Выполняет вход в состояние, освобождает точку у кассы и обрабатывает оплату. </summary>
        public override void Enter()
        {
            base.Enter();

            if (Context != null && Context.TryGet<ItemStack>(ContextType.PurchaseItem, out var itemStack))
            {
                bool playerInLocation = _locationManager.IsPlayerInLocation(LocationName.NewShop);
                _transactionService.SellToNpc(itemStack, playerInLocation);
                _spriteIndicator.HideSprite();
            }
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}