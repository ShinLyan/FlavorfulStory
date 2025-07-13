namespace FlavorfulStory.QuestSystem.TriggerActions
{
    /// <summary> Тип действия, выполняемого после завершения этапа квеста. </summary>
    public enum TriggerActionType
    {
        /// <summary> Выдаёт игроку новый квест. </summary>
        GiveQuest,

        /// <summary> Добавляет предмет в инвентарь игрока. </summary>
        GiveItem
    }
}