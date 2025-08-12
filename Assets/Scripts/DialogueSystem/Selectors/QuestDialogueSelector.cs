using FlavorfulStory.AI;
using FlavorfulStory.QuestSystem;
using FlavorfulStory.QuestSystem.Objectives;
using FlavorfulStory.QuestSystem.Objectives.Params;

namespace FlavorfulStory.DialogueSystem.Selectors
{
    /// <summary> Селектор диалогов, связанных с квестами. </summary>
    public class QuestDialogueSelector : IDialogueSelector
    {
        /// <summary> Контекст выполнения квестов. </summary>
        private readonly QuestExecutionContext _questExecutionContext;

        /// <summary> Инициализирует селектор с контекстом квестов. </summary>
        /// <param name="executionContext"> Контекст выполнения квестов. </param>
        public QuestDialogueSelector(QuestExecutionContext executionContext) =>
            _questExecutionContext = executionContext;

        /// <summary> Выбирает диалог для NPC, связанный с активными квестами. </summary>
        /// <param name="npcName"> Имя NPC. </param>
        /// <returns> Диалог квеста или null. </returns>
        public Dialogue SelectDialogue(NpcName npcName)
        {
            foreach (var questStatus in _questExecutionContext.QuestList.QuestStatuses)
            {
                int index = questStatus.CurrentStageIndex;
                if (index >= questStatus.Stages.Count) continue;

                var stage = questStatus.Stages[index];
                foreach (var objective in stage.Objectives)
                    if (!questStatus.IsObjectiveComplete(objective) &&
                        objective.Type == ObjectiveType.Talk)
                        return ((TalkObjectiveParams)objective.Params).Dialogue;
            }

            return null;
        }
    }
}