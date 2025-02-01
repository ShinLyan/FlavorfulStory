using System;
using UnityEngine;

namespace FlavorfulStory.Stats.CharacterStats
{
    /// <summary> Управляет уровнем здоровья персонажа и его изменениями. </summary>
    public class Health : BaseStat
    {
        /// <summary> Вызывается при изменении уровня здоровья. </summary>
        public event Action<float> OnHealthChanged;

        /// <summary> Увеличивает количество здоровья. </summary>
        /// <param name="healthToRestore"> Значение, на которое увеличивается здоровье. </param>
        public void Heal(float healthToRestore)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + healthToRestore, 0, MaxValue);
            OnHealthChanged?.Invoke(CurrentValue);
        }

        /// <summary> Уменьшает количество здоровья. </summary>
        /// <param name="damage"> Значение, на которое уменьшается здоровье. </param>
        public void TakeDamage(float damage)
        {
            CurrentValue = Mathf.Clamp(CurrentValue - damage, 0, MaxValue);
            OnHealthChanged?.Invoke(CurrentValue);
        }
    }
}