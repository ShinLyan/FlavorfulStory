using FlavorfulStory.InventorySystem;
using Zenject;

namespace FlavorfulStory.Economy
{
    /// <summary> Сервис транзакций, отвечающий за покупку и продажу предметов между игроком и NPC. </summary>
    public class TransactionService
    {
        /// <summary> Хранилище валюты игрока (кошелек). </summary>
        private readonly ICurrencyStorage _playerWallet;

        /// <summary> Хранилище валюты NPC (касса). </summary>
        private readonly ICurrencyStorage _cashRegister;

        /// <summary> Конструктор транзакционного сервиса с внедрением зависимостей. </summary>
        /// <param name="playerWallet"> Кошелек игрока. </param>
        /// <param name="cashRegister"> Касса NPC. </param>
        public TransactionService(
            [Inject(Id = "Player")] ICurrencyStorage playerWallet,
            [Inject(Id = "Register")] ICurrencyStorage cashRegister)
        {
            _playerWallet = playerWallet;
            _cashRegister = cashRegister;
        }

        /// <summary> NPC совершает покупку, деньги зачисляются в кассу. </summary>
        /// <param name="itemStack"> Продаваемый предмет и его количество. </param>
        public void ProcessNpcPurchase(ItemStack itemStack) =>
            _cashRegister.Add(itemStack.Item.SellPrice * itemStack.Number);

        /// <summary> Игрок пытается купить предмет, деньги списываются с кошелька. </summary>
        /// <param name="itemStack"> Покупаемый предмет и его количество. </param>
        /// <returns> true, если покупка успешна; иначе false. </returns>
        public bool TryProcessPlayerPurchase(ItemStack itemStack) =>
            _playerWallet.TrySpend(itemStack.Item.BuyPrice * itemStack.Number);

        /// <summary> Игрок забирает все деньги из кассы в свой кошелек. </summary>
        public void TransferRegisterToPlayer()
        {
            int amount = _cashRegister.Amount;
            if (_cashRegister.TrySpend(amount)) _playerWallet.Add(amount);
        }
    }
}