namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    /// <summary> Базовый класс параметров цели квеста. </summary>
    public abstract class ObjectiveParamsBase
    {
        /// <summary> Проверяет выполнение цели и помечает её завершённой, если условия соблюдены. </summary>
        /// <param name="questStatus"> Статус квеста, к которому относится цель. </param>
        /// <param name="context"> Контекст выполнения цели, содержащий доступ к инвентарю и квестам. </param>
        public abstract void CheckAndComplete(QuestStatus questStatus, ObjectiveExecutionContext context);
    }
}