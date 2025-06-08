using UnityEngine;

namespace FlavorfulStory.Stats
{
    /// <summary> Базовый класс для отображения атрибута, реализующий интерфейс IAttributeView. </summary>
    public abstract class BaseAttributeView : MonoBehaviour, IAttributeView
    {
        /// <summary> Обрабатывает изменение значения атрибута. </summary>
        /// <param name="currentValue"> Текущее значение атрибута. </param>
        /// <param name="delta"> Изменение значения по сравнению с предыдущим. </param>
        public abstract void HandleAttributeChange(float currentValue, float delta);

        /// <summary> Обрабатывает достижение нулевого значения атрибута. </summary>
        public abstract void HandleAttributeReachZero();

        /// <summary> Обрабатывает изменение максимального значения атрибута. </summary>
        /// <param name="currentValue"> Текущее значение атрибута. </param>
        /// <param name="maxValue"> Новое максимальное значение атрибута. </param>
        public abstract void HandleAttributeMaxValueChanged(float currentValue, float maxValue);

        /// <summary> Инициализирует отображение атрибута начальными значениями. </summary>
        /// <param name="currentValue"> Начальное текущее значение атрибута. </param>
        /// <param name="maxValue"> Начальное максимальное значение атрибута. </param>
        public abstract void Initialize(float currentValue, float maxValue);
    }
}