namespace FlavorfulStory.Control.CursorSystem
{
    /// <summary> Тип курсора. </summary>
    public enum CursorType
    {
        /// <summary> Курсор по умолчанию. </summary>
        Default,

        /// <summary> Курсор доступного диалога. </summary>
        DialogueAvailable,

        /// <summary> Курсор недоступного диалога. </summary>
        DialogueNotAvailable

        // Combat,
        // UI,
        // Pickup,
        // Shop
    }
}