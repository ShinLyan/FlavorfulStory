using System;
using UnityEngine;

namespace FlavorfulStory.Attributes
{
    /// <summary> Атрибут выносливости с поддержкой пассивной регенерации. </summary>
    public class StaminaAttribute : IRegenerableAttribute
    {
        /// <summary> Текущее значение атрибута. </summary>
        public float CurrentValue { get; private set; }

        /// <summary> Максимальное значение атрибута. </summary>
        public float MaxValue { get; private set; }

        /// <summary> Скорость восстановления значения в секунду. </summary>
        public float RegenRate { get; }

        /// <summary> Событие, вызываемое при изменении значения атрибута. </summary>
        public event Action<float, float> OnValueChanged;

        /// <summary> Событие, вызываемое при изменении максимального значения. </summary>
        public event Action<float, float> OnMaxValueChanged;

        /// <summary> Событие, вызываемое при достижении нулевого значения. </summary>
        public event Action OnReachedZero;

        /// <summary> Конструктор, задающий максимальное значение и скорость регенерации. </summary>
        /// <param name="max"> Максимальное значение выносливости. </param>
        /// <param name="regenRate"> Скорость восстановления выносливости в секунду. </param>
        public StaminaAttribute(float max, float regenRate)
        {
            MaxValue = max;
            CurrentValue = max;
            RegenRate = regenRate;
        }

        /// <summary> Изменяет текущее значение на заданную величину. </summary>
        /// <param name="delta"> Значение, на которое изменить атрибут. </param>
        public void Change(float delta)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + delta, 0, MaxValue);
            OnValueChanged?.Invoke(CurrentValue, delta);

            if (CurrentValue <= 0f)
                OnReachedZero?.Invoke();

            if (Mathf.Approximately(CurrentValue, MaxValue))
                OnMaxValueChanged?.Invoke(CurrentValue, MaxValue);
        }

        /// <summary> Устанавливает новое текущее значение атрибута. </summary>
        /// <param name="value"> Новое значение выносливости. </param>
        public void SetValue(float value)
        {
            float previous = CurrentValue;
            CurrentValue = Mathf.Clamp(value, 0, MaxValue);
            OnValueChanged?.Invoke(CurrentValue, CurrentValue - previous);
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

        /// <summary> Обновляет значение выносливости с учётом времени и скорости регенерации. </summary>
        /// <param name="deltaTime"> Время с момента последнего обновления. </param>
        public void TickRegen(float deltaTime)
        {
            if (CurrentValue < MaxValue)
                Change(RegenRate * deltaTime);
        }
    }
}