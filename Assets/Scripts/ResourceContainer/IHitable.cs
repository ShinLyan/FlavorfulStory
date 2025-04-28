using FlavorfulStory.Actions;
using FlavorfulStory.Audio;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Интерфейс объекта, который можно бить инструментом. </summary>
    public interface IHitable
    {
        /// <summary> Тип звука. </summary>
        SfxType SfxType { set; }

        /// <summary> Получить удар. </summary>
        /// <param name="toolType"> Тип инструмента, которым наносится удар. </param>
        public void TakeHit(ToolType toolType);
    }
}