namespace FlavorfulStory.DialogueSystem.Selectors
{
    /// <summary> Интерфейс для инициализируемых селекторов диалогов. </summary>
    public interface IInitializableSelector : IDialogueSelector
    {
        /// <summary> Инициализирует селектор. </summary>
        void Initialize();

        /// <summary> Освобождает ресурсы селектора. </summary>
        void Dispose();
    }
}