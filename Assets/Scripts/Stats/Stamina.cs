namespace FlavorfulStory.Stats
{
    /// <summary> Атрибут выносливости с поддержкой пассивной регенерации. </summary>
    public class Stamina : CharacterStat
    {
        /// <summary> Создать выносливость с максимальным значением, равным текущему. </summary>
        /// <param name="maxValue"> Максимальное значение выносливости. </param>
        public Stamina(float maxValue) : base(maxValue) { }

        /// <summary> Создать выносливость с заданными текущим и максимальным значениями. </summary>
        /// <param name="currentValue"> Текущее значение выносливости. </param>
        /// <param name="maxValue"> Максимальное значение выносливости. </param>
        public Stamina(float currentValue, float maxValue) : base(currentValue, maxValue) { }
    }
}