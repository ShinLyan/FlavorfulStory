using System;
using System.Linq;
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

        /// <summary> Проверяет выполнение условия разговора и завершает цель, если оно выполнено. </summary>
        /// <param name="questStatus"> Статус квеста. </param>
        /// <param name="context"> Контекст выполнения цели. </param>
        /// <param name="eventData"> Данные события, содержащие имя NPC и диалог. </param>
        public override void CheckAndComplete(QuestStatus questStatus, ObjectiveExecutionContext context,
            object eventData = null)
        {
            if (eventData is not (NpcName npcName, Dialogue dialogue)) return;

            if (npcName != NpcName || dialogue != Dialogue) return;

            var objective = questStatus.Quest.Objectives.FirstOrDefault(o => o.Params == this);
            if (objective != null)
                context.QuestList.CompleteObjective(questStatus, objective);
            else
                Debug.LogError($"Objective не найдено для Params {this} в квесте {questStatus.Quest.QuestName}!");
        }
    }
}