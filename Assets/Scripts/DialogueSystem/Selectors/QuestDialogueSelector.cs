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

        /// <summary> Выбирает подходящий диалог для NPC на основании текущих квестовых целей. </summary>
        /// <param name="npcInfo">Информация о NPC, для которого выбирается диалог.</param>
        /// <returns>Объект диалога или null, если подходящий диалог не найден.</returns>
        public Dialogue SelectDialogue(NpcInfo npcInfo)
        {
            foreach (var questStatus in _questExecutionContext.QuestList.QuestStatuses)
            foreach (var objective in questStatus.CurrentObjectives)
            {
                if (questStatus.IsObjectiveComplete(objective)) continue;

                if (objective is { Type: ObjectiveType.Talk, Params: TalkObjectiveParams talkParams })
                    return talkParams.Dialogue;
            }

            return null;
        }
    }
}