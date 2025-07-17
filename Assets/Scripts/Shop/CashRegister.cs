using System;
using FlavorfulStory.Economy;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.Shop
{
    /// <summary> Касса магазина, реализующая хранение валюты и сохранение её состояния. </summary>
    public class CashRegister : MonoBehaviour, ICurrencyStorage, ISaveable
    {
        /// <summary> Инициализирует значение золота при запуске. </summary>
        private void Start() => OnAmountChanged?.Invoke(Amount);

        #region ICurrencyStorage

        /// <summary> Текущее количество золота. </summary>
        public int Amount { get; private set; }

        /// <summary> Событие, вызываемое при изменении количества золота. </summary>
        public event Action<int> OnAmountChanged;

        /// <summary> Добавляет указанное количество золота. </summary>
        /// <param name="value"> Количество золота для добавления. </param>
        public void Add(int value)
        {
            Amount += value;
            OnAmountChanged?.Invoke(Amount);
        }

        /// <summary> Пытается потратить указанное количество золота. </summary>
        /// <param name="value"> Сумма, которую нужно потратить. </param>
        /// <returns> true, если золото было успешно потрачено; иначе — false. </returns>
        public bool TrySpend(int value)
        {
            if (Amount < value) return false;

            Amount -= value;
            OnAmountChanged?.Invoke(Amount);
            return true;
        }

        #endregion

        #region ISaveable

        /// <summary> Структура, представляющая сериализуемое состояние кассы. </summary>
        [Serializable]
        private struct RegisterData
        {
            /// <summary> Количество золота. </summary>
            public int Gold;
        }

        /// <summary> Сохраняет текущее состояние кассы. </summary>
        /// <returns> Объект состояния для сериализации. </returns>
        public object CaptureState() => new RegisterData { Gold = Amount };

        /// <summary> Восстанавливает состояние кассы из сериализованных данных. </summary>
        /// <param name="state"> Объект состояния, полученный при сохранении. </param>
        public void RestoreState(object state)
        {
            if (state is not RegisterData data) return;

            Amount = data.Gold;
        }

        #endregion
    }
}