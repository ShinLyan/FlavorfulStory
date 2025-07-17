using System;

namespace FlavorfulStory.Economy
{
    /// <summary> Интерфейс для компонентов, которые управляют хранилищем валюты. </summary>
    public interface ICurrencyStorage
    {
        /// <summary> Текущее количество доступной валюты. </summary>
        int Amount { get; }

        /// <summary> Событие, вызываемое при изменении количества валюты. </summary>
        event Action<int> OnAmountChanged;

        /// <summary> Добавляет указанное количество валюты в хранилище. </summary>
        /// <param name="value"> Сумма для добавления. </param>
        void Add(int value);

        /// <summary> Пытается потратить указанное количество валюты. </summary>
        /// <param name="value"> Сумма, которую требуется потратить. </param>
        /// <returns> <c>true</c>, если сумма успешно потрачена (достаточно средств), иначе <c>false</c>. </returns>
        bool TrySpend(int value);
    }
}