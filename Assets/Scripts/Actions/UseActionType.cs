using System;

namespace FlavorfulStory.Actions
{
    /// <summary> Тип действия, выполняемого при использовании объекта (например, клик мыши). </summary>
    [Serializable]
    public enum UseActionType
    {
        /// <summary> Левый клик мыши. </summary>
        LeftClick,

        /// <summary> Правый клик мыши. </summary>
        RightClick
    }
}