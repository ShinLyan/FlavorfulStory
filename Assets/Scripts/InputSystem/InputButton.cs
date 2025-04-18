namespace FlavorfulStory.InputSystem
{
    /// <summary> Перечисление кнопок и осей ввода в игре. </summary>
    public enum InputButton
    {
        /// <summary> Колесо прокрутки мыши. </summary>
        MouseScroll,

        /// <summary> Горизонтальная ось ввода. </summary>
        Horizontal,

        /// <summary> Вертикальная ось ввода. </summary> 
        Vertical,

        /// <summary> Переход к следующей реплике диалога. </summary>
        NextDialogue,

        /// <summary> Пропуск текущей реплики или всего диалога. </summary>
        SkipDialogue,

        /// <summary> Режим ходьбы. </summary>
        Walking,

        /// <summary> Переключение игрового меню. </summary> 
        SwitchGameMenu,

        /// <summary> Переключение на предыдущую вкладку. </summary>
        SwitchToPreviousTab,

        /// <summary> Переключение на следующую вкладку. </summary>
        SwitchToNextTab,

        /// <summary> Открыть главную вкладку в игровом меню. </summary>
        OpenMainTab,

        /// <summary> Открыть вкладку инвентаря в игровом меню. </summary>
        OpenInventoryTab,

        /// <summary> Открыть вкладку настроек в игровом меню. </summary>
        OpenSettingsTab,

        /// <summary> Взаимодействие с объектами. </summary>
        Interact,

        /// <summary> Позиция курсора мыши. </summary>
        MousePosition
    }
}