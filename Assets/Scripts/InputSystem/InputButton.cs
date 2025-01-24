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
        
        Interact,

        /// <summary> Позиция курсора мыши. </summary>
        MousePosition
    }
}