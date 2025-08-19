using Cysharp.Threading.Tasks;
using FlavorfulStory.Audio;
using FlavorfulStory.Economy;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.SceneManagement;

namespace FlavorfulStory.AI.FSM.ShopStates
{
    /// <summary> Состояние для обработки оплаты товаров у кассы. </summary>
    public class PaymentState : CharacterState
    {
        /// <summary> Менеджер локаций. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Сервис для обработки транзакций и торговых операций. </summary>
        private readonly TransactionService _transactionService;

        /// <summary> Индикатор купленного товара у НПС. </summary>
        private readonly NpcPurchaseIndicator _purchaseIndicator;

        /// <summary> Инициализирует новый экземпляр состояния оплаты. </summary>
        /// <param name="locationManager"> Локация магазина для доступа к кассе. </param>\
        /// <param name="transactionService"> Сервис для обработки транзакций. </param>
        /// <param name="purchaseIndicator"></param>
        public PaymentState(LocationManager locationManager, TransactionService transactionService,
            NpcPurchaseIndicator purchaseIndicator)
        {
            _locationManager = locationManager;
            _transactionService = transactionService;
            _purchaseIndicator = purchaseIndicator;
        }

        /// <summary> Выполняет вход в состояние, освобождает точку у кассы и обрабатывает оплату. </summary>
        public override void Enter()
        {
            if (Context == null || !Context.TryGet<ItemStack>(FsmContextType.PurchaseItem, out var itemStack)) return;

            if (_locationManager.IsPlayerInLocation(LocationName.NewShop)) SfxPlayer.Play(SfxType.Buy);
            _transactionService.ProcessNpcPurchase(itemStack);
            _purchaseIndicator.HideModel().Forget();
        }

        /// <summary> Возвращает статус завершения состояния. </summary>
        /// <returns> Всегда возвращает true, так как состояние завершается сразу после входа. </returns>
        public override bool IsComplete() => true;
    }
}