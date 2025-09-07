namespace FlavorfulStory.Toolbar
{
    /// <summary> Сигнал нажатия горячей клавиши панели быстрого доступа. </summary>
    public readonly struct ToolbarHotkeyPressedSignal
    {
        /// <summary> Индекс выбранного слота (0–9). </summary>
        public int SlotIndex { get; }

        /// <summary> Конструктор сигнала с индексом слота. </summary>
        /// <param name="slotIndex"> Индекс слота. </param>
        public ToolbarHotkeyPressedSignal(int slotIndex) => SlotIndex = slotIndex;
    }
}