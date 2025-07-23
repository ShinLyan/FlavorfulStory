using System;
using System.Collections.Generic;
using FlavorfulStory.QuestSystem.Objectives;
using FlavorfulStory.QuestSystem.Objectives.Params;
using FlavorfulStory.QuestSystem.TriggerActions;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Реестр соответствий типов целей и действий к их реализациям. </summary>
    public static class QuestRegistry
    {
        /// <summary> Сопоставление типов целей к соответствующим классам параметров. </summary>
        public static readonly Dictionary<ObjectiveType, Type> ObjectiveParamsMap = new()
        {
            { ObjectiveType.Have, typeof(HaveObjectiveParams) },
            { ObjectiveType.Talk, typeof(TalkObjectiveParams) },
            { ObjectiveType.Sleep, typeof(SleepObjectiveParams) },
            { ObjectiveType.Repair, typeof(RepairObjectiveParams) }
        };

        /// <summary> Сопоставление типов триггеров к соответствующим действиям. </summary>
        public static readonly Dictionary<TriggerActionType, Type> TriggerActionMap = new()
        {
            { TriggerActionType.GiveQuest, typeof(GiveQuestAction) },
            { TriggerActionType.GiveItem, typeof(GiveItemAction) }
        };
    }
}