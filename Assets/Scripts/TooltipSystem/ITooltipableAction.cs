using FlavorfulStory.Actions;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Объект, предоставляющий данные для тултипа. </summary>
    public interface ITooltipableAction
    {
        /// <summary> Описание действия, отображаемое в тултипе. </summary>
        ActionDescription ActionDescription { get; }
    }
}