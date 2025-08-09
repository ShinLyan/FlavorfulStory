using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.Toolbar
{
    /// <summary> Сигнал, отправляемый при выборе нового предмета в панели быстрого доступа. </summary>
    public readonly struct ToolbarSlotSelectedSignal
    {
        /// <summary> Выбранный предмет в панели быстрого доступа. </summary>
        public InventoryItem SelectedItem { get; }

        /// <summary> Конструктор сигнала с выбранным предметом панели быстрого доступа. </summary>
        /// <param name="selectedItem"> Выбранный предмет быстрого доступа. </param>
        public ToolbarSlotSelectedSignal(InventoryItem selectedItem) => SelectedItem = selectedItem;
    }
}