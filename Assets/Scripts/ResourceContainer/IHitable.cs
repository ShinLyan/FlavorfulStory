using FlavorfulStory.Actions;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Интерфейс объекта, который можно бить инструментов. </summary>
    public interface IHitable
    {
        /// <summary> Получить удар. </summary>
        /// <param name="toolType"> Тип инструмента, которым наносится удар. </param>
        void TakeHit(ToolType toolType);
    }
}