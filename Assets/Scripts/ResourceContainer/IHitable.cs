using FlavorfulStory.Actions;
using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Интерфейс объекта, который можно бить инструментом. </summary>
    public interface IHitable
    {
        /// <summary>
        /// Тип 
        /// </summary>
        SfxType SfxType { set; }

        /// <summary> Получить удар. </summary>
        /// <param name="toolType"> Тип инструмента, которым наносится удар. </param>
        public void TakeHit(ToolType toolType);
    }
}