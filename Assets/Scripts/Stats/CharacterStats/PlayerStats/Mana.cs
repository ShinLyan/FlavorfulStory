using System;
using FlavorfulStory.Stats.CharacterStats;
using UnityEngine;

namespace FlavorfulStory.Stats.PlayerStats
{
    /// <summary> Управляет запасом маны персонажа и её изменениями. </summary>
    public class Mana : BaseStat
    {
        /// <summary> Вызывается при изменении уровня маны. </summary>
        public event Action<float> OnManaChanged;
        
        /// <summary> Увеличивает количество маны. </summary>
        /// <param name="amount"> Значение, на которое увеличивается мана. </param>
        public void IncreaseMana(float amount)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + amount, 0, MaxValue);
            OnManaChanged?.Invoke(CurrentValue);
        }
        
        /// <summary> Уменьшает количество маны. </summary>
        /// <param name="amount"> Значение, на которое уменьшается мана. </param>
        public void DecreaseMana(float amount)
        {
            CurrentValue = Mathf.Clamp(CurrentValue - amount, 0, MaxValue);
            OnManaChanged?.Invoke(CurrentValue);
        }
    }
}