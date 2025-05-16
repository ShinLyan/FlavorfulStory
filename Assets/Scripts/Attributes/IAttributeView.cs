namespace FlavorfulStory.Attributes
{
    /// <summary> Интерфейс представления атрибута, обрабатывающего изменения значений. </summary>
    public interface IAttributeView
    {
        /// <summary> Обрабатывает изменение текущего значения атрибута. </summary>
        /// <param name="currentValue"> Новое текущее значение атрибута. </param>
        /// <param name="delta"> Изменение по сравнению с предыдущим значением. </param>
        void HandleAttributeChange(float currentValue, float delta);

        /// <summary> Обрабатывает событие достижения атрибутом нулевого значения. </summary>
        void HandleAttributeReachZero();

        /// <summary> Обрабатывает изменение максимального значения атрибута. </summary>
        /// <param name="currentValue"> Текущее значение атрибута. </param>
        /// <param name="maxValue"> Новое максимальное значение атрибута. </param>
        void HandleAttributeMaxValueChanged(float currentValue, float maxValue);

        /// <summary> Инициализирует представление начальными значениями атрибута. </summary>
        /// <param name="currentValue"> Начальное текущее значение. </param>
        /// <param name="maxValue"> Начальное максимальное значение. </param>
        void Initialize(float currentValue, float maxValue);
    }
}