using System;
using FlavorfulStory.AI;
using FlavorfulStory.DialogueSystem;
using UnityEngine;

namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    /// <summary> Параметры цели на разговор с определённым NPC по заданному диалогу. </summary>
    [Serializable]
    public class TalkObjectiveParams : ObjectiveParamsBase
    {
        /// <summary> Имя NPC, с которым нужно поговорить. </summary>
        [field: SerializeField] public NpcName NpcName { get; private set; }

        /// <summary> Диалог, который должен быть использован в разговоре. </summary>
        [field: SerializeField] public Dialogue Dialogue { get; private set; }

        /// <summary> Проверяет, соответствует ли разговор нужному NPC и диалогу. </summary>
        /// <param name="context"> Контекст выполнения цели. </param>
        /// <param name="eventData"> Событие, содержащее имя NPC и диалог. </param>
        /// <returns> True, если цель выполнена. </returns>
        protected override bool ShouldComplete(QuestExecutionContext context, object eventData)
        {
            if (eventData is not (NpcName npcName, Dialogue dialogue)) return false;
            return npcName == NpcName && dialogue == Dialogue;
        }
    }
}