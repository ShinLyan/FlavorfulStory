using System;
using UnityEngine;

namespace FlavorfulStory.Stats
{
    /// <summary> Базовый класс изменяемого состояния персонажа, например, здоровья или выносливости. </summary>
    public abstract class CharacterStat
    {
        /// <summary> Текущее значение параметра. </summary>
        public float CurrentValue { get; protected set; }

        /// <summary> Максимальное значение параметра. </summary>
        public float MaxValue { get; protected set; }

        /// <summary> Событие, вызываемое при изменении текущего значения. </summary>
        public event Action<float, float> OnValueChanged;

        /// <summary> Событие, вызываемое при изменении максимального значения. </summary>
        public event Action<float, float> OnMaxValueChanged;

        /// <summary> Событие, вызываемое при достижении значения нуля. </summary>
        public event Action OnReachedZero;

        /// <summary> Создать новое состояние с заданным максимумом и (опционально) текущим значением. </summary>
        /// <param name="maxValue"> Максимальное значение. </param>
        /// <param name="currentValue"> Начальное текущее значение. Если не указано — приравнивается максимуму. </param>
        protected CharacterStat(float maxValue, float? currentValue = null)
        {
            MaxValue = Mathf.Max(0, maxValue);
            CurrentValue = Mathf.Clamp(currentValue ?? maxValue, 0, MaxValue);
        }

        /// <summary> Изменить текущее значение на заданное смещение. </summary>
        /// <param name="delta"> Величина изменения (может быть отрицательной). </param>
        public virtual void Change(float delta)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + delta, 0, MaxValue);
            OnValueChanged?.Invoke(CurrentValue, delta);

            if (Mathf.Approximately(CurrentValue, 0)) OnReachedZero?.Invoke();
        }

        /// <summary> Установить конкретное значение, ограниченное диапазоном от 0 до максимума. </summary>
        /// <param name="newValue"> Новое значение. </param>
        public virtual void SetValue(float newValue)
        {
            float prev = CurrentValue;
            CurrentValue = Mathf.Clamp(newValue, 0, MaxValue);
            OnValueChanged?.Invoke(CurrentValue, CurrentValue - prev);
        }

        /// <summary> Установить новое максимальное значение. Корректирует текущее значение при необходимости. </summary>
        /// <param name="newMaxValue"> Новое максимальное значение. </param>
        public virtual void SetMaxValue(float newMaxValue)
        {
            MaxValue = newMaxValue;
            CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);
            OnMaxValueChanged?.Invoke(CurrentValue, MaxValue);
        }
    }
}