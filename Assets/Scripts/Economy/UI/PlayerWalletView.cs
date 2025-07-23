using TMPro;
using UnityEngine;

namespace FlavorfulStory.Economy
{
    /// <summary> Отображение количества валюты игрока в UI. </summary>
    public class PlayerWalletView : MonoBehaviour
    {
        /// <summary> Текстовое поле для отображения текущего количества золота. </summary>
        [SerializeField] private TMP_Text _amountText;

        /// <summary> Привязывает представление к валюте и обновляет отображение при изменении. </summary>
        /// <param name="storage"> Источник данных валюты (например, кошелек игрока). </param>
        public void Bind(ICurrencyStorage storage) =>
            storage.OnAmountChanged += newAmount => _amountText.text = newAmount.ToString();
    }
}