using System;
using FlavorfulStory.AI;
using UnityEngine;

namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    [Serializable]
    public class TalkObjectiveParams : ObjectiveParamsBase
    {
        [field: SerializeField] public NpcName NpcName { get; private set; }

        public override void CheckAndComplete(QuestStatus questStatus, ObjectiveExecutionContext context)
        {
            throw new NotImplementedException();
            // questList.CompleteObjective(questStatus.Quest, questStatus.QuestName);
        }
    }
}