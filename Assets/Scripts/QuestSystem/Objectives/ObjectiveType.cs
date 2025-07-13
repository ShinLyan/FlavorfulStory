namespace FlavorfulStory.QuestSystem.Objectives
{
    /// <summary> Тип цели квеста. </summary>
    public enum ObjectiveType
    {
        /// <summary> Иметь указанное количество предметов. </summary>
        Have,

        /// <summary> Поговорить с NPC. </summary>
        Talk,

        /// <summary> Поспать. </summary>
        Sleep,

        /// <summary> Отремонтировать здание. </summary>
        Repair

        // Craft,          // Скрафтить
        // Deliver,        // Принести
        // Remove,         // Удалить
        // Kill,           // Убить
        // Reach,          // Дойти
        // Place,          // Разместить товар
        // Use             // Использовать колокольчик
    }
}