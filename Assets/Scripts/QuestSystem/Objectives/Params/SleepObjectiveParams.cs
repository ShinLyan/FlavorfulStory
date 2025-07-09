namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    /// <summary> Параметры цели на сон — цель завершается автоматически при завершении дня. </summary>
    public class SleepObjectiveParams : ObjectiveParamsBase
    {
        /// <summary> Условие всегда возвращает true — цель считается выполненной при любом вызове. </summary>
        /// <param name="status"> Статус квеста. </param>
        /// <param name="context"> Контекст выполнения цели. </param>
        /// <param name="eventData"> Данные события (не используются). </param>
        /// <returns> True — цель выполняется без условий. </returns>
        protected override bool ShouldComplete(QuestStatus status, ObjectiveExecutionContext context, object eventData)
            => true;
    }
}