namespace FlavorfulStory.Attributes
{
    /// <summary> Атрибут выносливости с поддержкой пассивной регенерации. </summary>
    public class StaminaAttribute : BaseAttribute, IRegenerableAttribute
    {
        public float RegenRate { get; }

        public StaminaAttribute(float max, float regenRate, float current = -1) : base(max)
        {
            RegenRate = regenRate;
            CurrentValue = current < 0 ? max : current;
        }

        public void TickRegen(float deltaTime)
        {
            if (CurrentValue < MaxValue)
                Change(RegenRate * deltaTime);
        }
    }
}