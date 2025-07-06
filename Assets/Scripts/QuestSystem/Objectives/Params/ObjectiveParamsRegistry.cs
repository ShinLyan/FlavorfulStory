using System;
using System.Collections.Generic;

namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    /// <summary> Регистратор типов параметров целей квестов по их типу. </summary>
    public static class ObjectiveParamsRegistry
    {
        /// <summary> Словарь, сопоставляющий тип цели и соответствующий класс параметров. </summary>
        public static readonly Dictionary<ObjectiveType, Type> Mapping = new()
        {
            { ObjectiveType.Collect, typeof(CollectObjectiveParams) },
            { ObjectiveType.Talk, typeof(TalkObjectiveParams) }
        };
    }
}