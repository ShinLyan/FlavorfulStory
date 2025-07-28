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
        public TransactionService([Inject(Id = "Player")] ICurrencyStorage playerWallet,
            [Inject(Id = "Register")] ICurrencyStorage cashRegister)
        {
            _playerWallet = playerWallet;
            _cashRegister = cashRegister;
        }

        /// <summary> Продает предмет NPC и переводит деньги в кассу. </summary>
        /// <param name="itemStack"> Продаваемый предмет и его количество. </param>
        public void SellToNpc(ItemStack itemStack)
        {
            int total = itemStack.Item.SellPrice * itemStack.Number;
            _cashRegister.Add(total); // Деньги идут в кассу
        }

        /// <summary> Пытается купить предмет у NPC и списывает деньги с кошелька игрока. </summary>
        /// <param name="itemStack"> Покупаемый предмет и его количество. </param>
        /// <returns> true, если покупка успешна; иначе false. </returns>
        public bool TryBuyFromNpc(ItemStack itemStack)
        {
            int cost = itemStack.Item.BuyPrice * itemStack.Number;
            return _playerWallet.TrySpend(cost);
        }

        /// <summary> Перевести деньги из кассы игроку. </summary>
        public void TransferMoneyFromCashRegisterToPlayer()
        {
            _playerWallet.Add(_cashRegister.Amount);
            _cashRegister.TrySpend(_cashRegister.Amount);
        }
    }
}