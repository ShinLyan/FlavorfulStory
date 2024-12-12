using System;
using UnityEngine;

namespace FlavorfulStory.Stats.CharacterStats
{
    /// <summary> Класс, обеспечивающий взаимодействие со здоровьем персонажа.</summary>
    public class Health : BaseStat
    {
        /// <summary> Событие изменения здоровья.</summary>
        public event Action<float> OnHealthChanged;
        
        /// <summary> Метод, вызываемый для увеличения здоровья.</summary>
        /// <param name="healthToRestore"> Значение, на которое увеличивается здоровье.</param>
        public void Heal(float healthToRestore)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + healthToRestore, 0, MaxValue);

            OnHealthChanged?.Invoke(CurrentValue);
        }
        
        /// <summary> Метод, вызываемый для уменьшения здоровья.</summary>
        /// <param name="damage"> Значение, на которое уменьшается здоровье.</param>
        public void TakeDamage(float damage)
        {
            CurrentValue = Mathf.Clamp(CurrentValue - damage, 0, MaxValue);

            OnHealthChanged?.Invoke(CurrentValue);
        }
    }
}