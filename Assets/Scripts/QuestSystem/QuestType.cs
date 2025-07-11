namespace FlavorfulStory.QuestSystem
{
    /// <summary> Тип квеста, определяющий его назначение и периодичность. </summary>
    public enum QuestType
    {
        /// <summary> Общий сюжетный квест. </summary>
        General,

        /// <summary> Персональный квест, связанный с конкретным персонажем. </summary>
        Personal,

        /// <summary> Ежедневный квест, обновляется каждый день. </summary>
        Daily,

        /// <summary> Еженедельный квест, обновляется раз в неделю. </summary>
        Weekly
    }
}