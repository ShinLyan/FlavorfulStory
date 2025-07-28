using System;
using UnityEngine;

namespace FlavorfulStory.Stats
{
    /// <summary> Базовый класс изменяемого состояния персонажа, например, здоровья или выносливости. </summary>
    public abstract class CharacterStat
    {
        /// <summary> Текущее значение параметра. </summary>
        public float CurrentValue { get; private set; }

        /// <summary> Максимальное значение параметра. </summary>
        public float MaxValue { get; private set; }

        /// <summary> Событие, вызываемое при изменении текущего значения. </summary>
        public event Action<float> OnValueChanged;

        /// <summary> Событие, вызываемое при изменении максимального значения. </summary>
        public event Action<float> OnMaxValueChanged;

        /// <summary> Событие, вызываемое при достижении значения нуля. </summary>
        public event Action OnReachedZero;

        /// <summary> Создать параметр со значением, равным максимуму. </summary>
        /// <param name="maxValue"> Максимальное значение. </param>
        protected CharacterStat(float maxValue) : this(maxValue, maxValue) { }

        /// <summary> Создать параметр с указанными текущим и максимальным значениями. </summary>
        /// <param name="currentValue"> Начальное текущее значение. </param>
        /// <param name="maxValue"> Максимальное значение. </param>
        protected CharacterStat(float currentValue, float maxValue)
        {
            CurrentValue = currentValue;
            MaxValue = maxValue;
        }

        /// <summary> Изменить текущее значение на заданное смещение. </summary>
        /// <param name="delta"> Величина изменения (может быть отрицательной). </param>
        public void ChangeValue(float delta)
        {
            float newValue = Mathf.Clamp(CurrentValue + delta, 0, MaxValue);
            ApplyValueChange(newValue);
        }

        /// <summary> Применить обновлённое значение и вызвать соответствующие события. </summary>
        /// <param name="newValue"> Новое значение. </param>
        private void ApplyValueChange(float newValue)
        {
            CurrentValue = newValue;
            OnValueChanged?.Invoke(CurrentValue);

            if (Mathf.Approximately(CurrentValue, 0f)) OnReachedZero?.Invoke();
        }

        /// <summary> Установить конкретное значение, ограниченное диапазоном от 0 до максимума. </summary>
        /// <param name="newValue"> Новое значение. </param>
        private void SetValue(float newValue)
        {
            float clamped = Mathf.Clamp(newValue, 0, MaxValue);
            ApplyValueChange(clamped);
        }

        /// <summary> Установить новое максимальное значение. </summary>
        /// <param name="newMaxValue"> Новое максимальное значение. </param>
        public void SetMaxValue(float newMaxValue)
        {
            MaxValue = Mathf.Max(0, newMaxValue);
            OnMaxValueChanged?.Invoke(MaxValue);
        }

        /// <summary> Восстановить полное значение параметра. </summary>
        public void RestoreFull() => SetValue(MaxValue);

        /// <summary> Восстановить процент от максимального значения параметра. </summary>
        /// <param name="percent"> Процент от максимального значения, которое нужно восстановить. </param>
        public void RestorePercent(float percent)
        {
            float clampedPercent = Mathf.Clamp01(percent);
            SetValue(MaxValue * clampedPercent);
        }
    }
}