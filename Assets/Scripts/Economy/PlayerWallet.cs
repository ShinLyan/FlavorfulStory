using System;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Economy
{
    /// <summary> Кошелек игрока, реализующий хранение и изменение количества валюты,
    /// а также возможность сохранения и восстановления состояния. </summary>
    public class PlayerWallet : MonoBehaviour, ICurrencyStorage, ISaveable
    {
        /// <summary> Стартовое количество золота у игрока. </summary>
        private const int StartingGold = 1000;

        /// <summary> Внедряет визуальное представление кошелька и привязывает его к этому компоненту. </summary>
        /// <param name="view"> Представление кошелька игрока. </param>
        [Inject]
        private void Construct(PlayerWalletView view) => view.Bind(this);

        /// <summary> Инициализация кошелька: уведомляет подписчиков о текущем балансе. </summary>
        private void Start() => OnAmountChanged?.Invoke(Amount);

        #region ICurrencyStorage

        /// <summary> Текущее количество золота у игрока. </summary>
        public int Amount { get; private set; } = StartingGold;

        /// <summary> Событие, вызываемое при изменении количества золота. </summary>
        public event Action<int> OnAmountChanged;

        /// <summary> Добавляет указанное количество золота. </summary>
        /// <param name="value"> Сумма для добавления. </param>
        public void Add(int value)
        {
            Amount += value;
            OnAmountChanged?.Invoke(Amount);
        }

        /// <summary> Пытается потратить указанное количество золота. </summary>
        /// <param name="value"> Сумма, которую нужно потратить. </param>
        /// <returns> <c>true</c> — если достаточно средств; иначе <c>false</c>. </returns>
        public bool TrySpend(int value)
        {
            if (Amount < value) return false;

            Amount -= value;
            OnAmountChanged?.Invoke(Amount);
            return true;
        }

        #endregion

        #region ISaveable

        /// <summary> Сохраняет текущее состояние кошелька. </summary>
        /// <returns> Сериализуемое состояние. </returns>
        public object CaptureState() => Amount;

        /// <summary> Восстанавливает состояние кошелька из сериализованных данных. </summary>
        /// <param name="state"> Объект состояния, содержащий количество золота. </param>
        public void RestoreState(object state) => Amount = (int)state;

        #endregion
    }
}