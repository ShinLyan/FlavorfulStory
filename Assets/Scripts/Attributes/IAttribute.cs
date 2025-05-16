using System;

namespace FlavorfulStory.Attributes
{
    /// <summary> Интерфейс атрибута с возможностью изменения значений и подписки на события. </summary>
    public interface IAttribute
    {
        /// <summary> Текущее значение атрибута. </summary>
        float CurrentValue { get; }

        /// <summary> Максимальное значение атрибута. </summary>
        float MaxValue { get; }

        /// <summary> Изменяет текущее значение атрибута на указанное смещение. </summary>
        /// <param name="delta"> Значение, на которое нужно изменить атрибут. </param>
        void Change(float delta);

        /// <summary> Устанавливает новое текущее значение атрибута. </summary>
        /// <param name="value"> Новое значение атрибута. </param>
        void SetValue(float value);

        /// <summary> Устанавливает новое максимальное значение атрибута. </summary>
        /// <param name="max"> Новое максимальное значение. </param>
        void SetMaxValue(float max);

        /// <summary> Событие, вызываемое при инициализации атрибута. </summary>
        event Action OnInitialized;

        /// <summary> Событие, вызываемое при изменении текущего значения. </summary>
        event Action<float, float> OnValueChanged;

        /// <summary> Событие, вызываемое при изменении максимального значения. </summary>
        event Action<float, float> OnMaxValueChanged;

        /// <summary> Событие, вызываемое при достижении нулевого значения. </summary>
        event Action OnReachedZero;
    }
}