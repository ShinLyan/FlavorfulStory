namespace FlavorfulStory.Stats
{
    /// <summary> Реализация атрибута здоровья, поддерживающего события изменения значений. </summary>
    public class Health : CharacterStat
    {
        public Health(float maxValue, float? currentValue = null) : base(maxValue, currentValue) { }
    }
}