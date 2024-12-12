using System;
using FlavorfulStory.Stats.CharacterStats;
using UnityEngine;

namespace FlavorfulStory.Stats.PlayerStats
{
    /// <summary> Класс, обеспечивающий взаимодействие с энергией персонажа.</summary>
    public class Mana : BaseStat
    {
        /// <summary> Событие, вызываемое при изменении маны.</summary>
        public event Action<float> OnManaChanged;
        
        /// <summary>  Метод, вызываемый при увеличении маны.</summary>
        /// <param name="amount">Значение, на которое увеличивается мана.</param>
        public void IncreaseMana(float amount)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + amount, 0, MaxValue);

            OnManaChanged?.Invoke(CurrentValue);
        }
        
        /// <summary> Метод, вызываемый при уменьшении маны.</summary>
        /// <param name="amount">Значение, на которое уменьшается мана.</param>
        public void DecreaseMana(float amount)
        {
            CurrentValue = Mathf.Clamp(CurrentValue - amount, 0, MaxValue);

            OnManaChanged?.Invoke(CurrentValue);
        }
    }
}