namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    /// <summary> Базовый класс параметров цели квеста. </summary>
    public abstract class ObjectiveParamsBase
    {
        /// <summary> Проверяет выполнение цели и помечает её завершённой, если условия соблюдены. </summary>
        /// <param name="status"> Статус квеста, к которому относится цель. </param>
        /// <param name="context"> Контекст выполнения цели, содержащий доступ к инвентарю и квестам. </param>
        /// <param name="eventData"> Данные события, которые необходимы для конкретных реализаций. </param>
        public abstract void CheckAndComplete(QuestStatus status, ObjectiveExecutionContext context,
            object eventData = null);
    }
}