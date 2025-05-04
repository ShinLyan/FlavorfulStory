using System;
using UnityEngine;

namespace FlavorfulStory.Attributes
{
    public class BaseAttribute : IAttribute
    {
        public float CurrentValue { get; protected set; }
        public float MaxValue { get; protected set; }

        public event Action OnInitialized;
        public event Action<float, float> OnValueChanged;
        public event Action<float, float> OnMaxValueChanged;
        public event Action OnReachedZero;

        protected BaseAttribute(float max)
        {
            MaxValue = max;
            CurrentValue = max;
        }

        public virtual void Change(float delta)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + delta, 0, MaxValue);
            OnValueChanged?.Invoke(CurrentValue, delta);

            if (CurrentValue <= 0f)
                OnReachedZero?.Invoke();

            if (Mathf.Approximately(CurrentValue, MaxValue))
                OnMaxValueChanged?.Invoke(CurrentValue, MaxValue);
        }

        public virtual void SetValue(float value)
        {
            float prev = CurrentValue;
            CurrentValue = Mathf.Clamp(value, 0, MaxValue);
            OnValueChanged?.Invoke(CurrentValue, CurrentValue - prev);
        }

        public virtual void SetMaxValue(float max)
        {
            MaxValue = max;
            CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);
            OnMaxValueChanged?.Invoke(CurrentValue, MaxValue);
        }

        public void Initialize() => OnInitialized?.Invoke();
    }
}