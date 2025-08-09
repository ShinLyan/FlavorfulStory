using FlavorfulStory.Audio;
using FlavorfulStory.Tools;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Интерфейс объекта, который можно бить инструментом. </summary>
    public interface IHitable
    {
        /// <summary> Тип инструмента, необходимый для взаимодействия (например, топор, кирка). </summary>
        ToolType RequiredToolType { get; }

        /// <summary> Минимальный уровень инструмента, необходимый для нанесения урона. </summary>
        int RequiredToolLevel { get; }

        /// <summary> Тип звукового эффекта, проигрываемого при ударе. </summary>
        SfxType SfxType { get; }

        /// <summary> Может ли указанный инструмент нанести урон этому объекту? </summary>
        /// <param name="toolType"> Тип инструмента. </param>
        /// <param name="toolLevel"> Уровень инструмента. </param>
        /// <returns> <c>true</c>, если удар возможен; иначе <c>false</c>. </returns>
        bool CanBeHitBy(ToolType toolType, int toolLevel);

        /// <summary> Применяет удар к объекту. </summary>
        void TakeHit();
    }
}