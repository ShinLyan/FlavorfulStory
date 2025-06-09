namespace FlavorfulStory.Stats
{
    /// <summary> Реализация атрибута здоровья, поддерживающего события изменения значений. </summary>
    public class Health : CharacterStat
    {
        /// <summary> Создать здоровье с максимальным значением, равным текущему. </summary>
        /// <param name="maxValue"> Максимальное значение здоровья. </param>
        public Health(float maxValue) : base(maxValue) { }

        /// <summary> Создать здоровье с заданными текущим и максимальным значениями. </summary>
        /// <param name="currentValue"> Текущее значение здоровья. </param>
        /// <param name="maxValue"> Максимальное значение здоровья. </param>
        public Health(float currentValue, float maxValue) : base(currentValue, maxValue) { }
    }
}