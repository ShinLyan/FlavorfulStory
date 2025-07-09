using UnityEngine;

namespace FlavorfulStory.EditorTools.Attributes
{
    /// <summary> Атрибут для отображения слайдера с шагом в инспекторе. </summary>
    public class SteppedRangeAttribute : PropertyAttribute
    {
        /// <summary> Минимальное значение диапазона. </summary>
        public float Min { get; }

        /// <summary> Максимальное значение диапазона. </summary>
        public float Max { get; }

        /// <summary> Шаг изменения значения. </summary>
        public float Step { get; }

        /// <summary> Конструктор атрибута SteppedRange. </summary>
        /// <param name="min"> Минимальное значение. </param>
        /// <param name="max"> Максимальное значение. </param>
        /// <param name="step"> Шаг между значениями. </param>
        public SteppedRangeAttribute(float min, float max, float step)
        {
            Min = min;
            Max = max;
            Step = step;
        }
    }
}