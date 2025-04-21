using UnityEngine;

namespace FlavorfulStory.Utils.Editor.EnumFlags
{
    /// <summary> Атрибут для отображения enum с флагами в виде MaskField в инспекторе. </summary>
    /// <remarks> Используется в сочетании с <see cref="EnumFlagsAttributeDrawer"/> для кастомной отрисовки. </remarks>
    public class EnumFlagsAttribute : PropertyAttribute
    {
        // Этот атрибут сам по себе ничего не делает, но используется для маркировки enum-полей,
        // чтобы EnumFlagsAttributeDrawer мог их кастомно отрисовывать.
    }
}