using System;
using FlavorfulStory.Stats;
using FlavorfulStory.Stats.CharacterStats;
using UnityEngine;

namespace FlavorfulStory.Stats.PlayerStats
{
    /// <summary>  Класс, обеспечивающий взаимодействие с выносливостью персонажа. </summary>
    public class Stamina : BaseStat
    {
        /// <summary> Событие изменения выносливости. </summary>
        public event Action<float> OnStaminaChanged;

        /// <summary> Увеличение выносливости. </summary>
        /// <param name="amount"> Значение, на которое увеличивается выносливость. </param>
        public void IncreaseStamina(float amount)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + amount, 0, MaxValue);

            OnStaminaChanged?.Invoke(CurrentValue);
        }

        /// <summary> Уменьшение выносливости. </summary>
        /// <param name="amount"> Значение, на которое уменьшается выносливость. </param>
        public void DecreaseStamina(float amount)
        {
            CurrentValue = Mathf.Clamp(CurrentValue - amount, 0, MaxValue);

            OnStaminaChanged?.Invoke(CurrentValue);
        }
    }
}