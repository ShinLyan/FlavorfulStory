using System.Linq;

namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    /// <summary> Базовый класс параметров цели квеста. </summary>
    public abstract class ObjectiveParamsBase
    {
        /// <summary> Проверяет выполнение цели и помечает её завершённой, если условия соблюдены. </summary>
        /// <param name="status"> Статус квеста, к которому относится цель. </param>
        /// <param name="context"> Контекст выполнения цели, содержащий доступ к инвентарю и квестам. </param>
        /// <param name="eventData"> Данные события, которые необходимы для конкретных реализаций. </param>
        public void CheckAndComplete(QuestStatus status, ObjectiveExecutionContext context, object eventData = null)
        {
            if (ShouldComplete(context, eventData)) CompleteObjective(status, context);
        }

        /// <summary> Проверяет, выполнены ли условия завершения цели. </summary>
        /// <param name="context"> Контекст выполнения цели. </param>
        /// <param name="eventData"> Данные события. </param>
        /// <returns> True, если цель должна быть завершена. </returns>
        protected abstract bool ShouldComplete(ObjectiveExecutionContext context, object eventData);

        /// <summary> Завершает цель, если она найдена в текущем квесте. </summary>
        /// <param name="status"> Статус квеста. </param>
        /// <param name="context"> Контекст выполнения цели. </param>
        private void CompleteObjective(QuestStatus status, ObjectiveExecutionContext context)
        {
            var objective = status.CurrentObjectives.SingleOrDefault(questObjective => questObjective.Params == this);
            if (objective != null) context.QuestList.CompleteObjective(status, objective);
        }
    }
}