namespace FlavorfulStory.Attributes
{
    /// <summary> Интерфейс атрибута с пассивной регенерацией. </summary>
    public interface IRegenerableAttribute : IAttribute
    {
        /// <summary> Скорость восстановления значения в секунду. </summary>
        float RegenRate { get; }

        /// <summary> Вызывает регенерацию значения в зависимости от прошедшего времени. </summary>
        /// <param name="deltaTime"> Время в секундах с момента последнего обновления. </param>
        void TickRegen(float deltaTime);
    }
}