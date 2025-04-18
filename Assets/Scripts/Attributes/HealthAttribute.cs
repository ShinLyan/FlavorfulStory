using System;
using UnityEngine;

namespace FlavorfulStory.Attributes
{
    /// <summary> Реализация атрибута здоровья, поддерживающего события изменения значений. </summary>
    public class HealthAttribute : IAttribute
    {
        /// <summary> Текущее значение атрибута. </summary>
        public float CurrentValue { get; private set; }

        /// <summary> Максимальное значение атрибута. </summary>
        public float MaxValue { get; private set; }

        /// <summary> Событие, вызываемое при изменении значения атрибута. </summary>
        public event Action<float, float> OnValueChanged;

        /// <summary> Событие, вызываемое при достижении максимального значения. </summary>
        public event Action<float, float> OnMaxValueChanged;

        /// <summary> Событие, вызываемое при достижении нулевого значения. </summary>
        public event Action OnReachedZero;

        /// <summary> Конструктор, инициализирующий атрибут заданным максимальным значением. </summary>
        /// <param name="max"> Начальное максимальное значение атрибута. </param>
        public HealthAttribute(float max)
        {
            MaxValue = max;
            CurrentValue = max;
        }

        /// <summary> Изменяет текущее значение на заданное смещение. </summary>
        /// <param name="delta"> Величина изменения значения. </param>
        public void Change(float delta)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + delta, 0, MaxValue);
            OnValueChanged?.Invoke(CurrentValue, delta);

            if (CurrentValue <= 0f)
                OnReachedZero?.Invoke();

            if (Mathf.Approximately(CurrentValue, MaxValue))
                OnMaxValueChanged?.Invoke(CurrentValue, MaxValue);
        }

        /// <summary> Устанавливает текущее значение атрибута. </summary>
        /// <param name="value"> Новое значение атрибута. </param>
        public void SetValue(float value)
        {
            float previousValue = CurrentValue;
            CurrentValue = Mathf.Clamp(value, 0, MaxValue);
            float delta = CurrentValue - previousValue;
            OnValueChanged?.Invoke(CurrentValue, delta);
        }

        /// <summary> Устанавливает новое максимальное значение атрибута. </summary>
        /// <param name="max"> Новое максимальное значение. </param>
        public void SetMaxValue(float max)
        {
            MaxValue = max;
            if (CurrentValue > MaxValue)
                SetValue(MaxValue);
            OnMaxValueChanged?.Invoke(CurrentValue, MaxValue);
        }
    }
}