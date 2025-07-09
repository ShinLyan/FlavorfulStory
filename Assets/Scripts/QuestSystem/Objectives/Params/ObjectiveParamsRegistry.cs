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
            { ObjectiveType.Have, typeof(HaveObjectiveParams) },
            { ObjectiveType.Talk, typeof(TalkObjectiveParams) },
            { ObjectiveType.Sleep, typeof(SleepObjectiveParams) },
            { ObjectiveType.Repair, typeof(RepairObjectiveParams) }
        };
    }
}