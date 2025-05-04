namespace FlavorfulStory.Attributes
{
    /// <summary> Реализация атрибута здоровья, поддерживающего события изменения значений. </summary>
    public class HealthAttribute : BaseAttribute
    {
        public HealthAttribute(float max) : base(max) { }
        
        public HealthAttribute(float max, float current = -1) : base(max)
        {
            CurrentValue = current < 0 ? max : current;
        }
    }
}