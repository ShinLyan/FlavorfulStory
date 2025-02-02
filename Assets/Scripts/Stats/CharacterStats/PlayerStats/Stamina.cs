using System;
using FlavorfulStory.Stats;
using FlavorfulStory.Stats.CharacterStats;
using UnityEngine;

namespace FlavorfulStory.Stats.PlayerStats
{
    /// <summary> Управляет запасом выносливости персонажа и её изменениями. </summary>
    public class Stamina : BaseStat
    {
        /// <summary> Вызывается при изменении уровня выносливости. </summary>
        public event Action<float> OnStaminaChanged;

        /// <summary> Увеличивает количество выносливости. </summary>
        /// <param name="amount"> Значение, на которое увеличивается выносливость. </param>
        public void IncreaseStamina(float amount)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + amount, 0, MaxValue);
            OnStaminaChanged?.Invoke(CurrentValue);
        }

        /// <summary> Уменьшает количество выносливости. </summary>
        /// <param name="amount"> Значение, на которое уменьшается выносливость. </param>
        public void DecreaseStamina(float amount)
        {
            CurrentValue = Mathf.Clamp(CurrentValue - amount, 0, MaxValue);
            OnStaminaChanged?.Invoke(CurrentValue);
        }
    }
}