namespace FlavorfulStory.Stats
{
    /// <summary> Атрибут выносливости с поддержкой пассивной регенерации. </summary>
    public class Stamina : CharacterStat
    {
        public Stamina(float maxValue, float? currentValue = null) : base(maxValue, currentValue) { }
    }
}