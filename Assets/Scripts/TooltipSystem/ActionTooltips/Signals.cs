using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.TooltipSystem.ActionTooltips
{
    /// <summary> Сигнал, отправляемый при выборе слота на тулбаре. </summary>
    public struct ToolbarSlotSelectedSignal
    {
        /// <summary> Предмет, выбранный в слоте тулбара. </summary>
        public InventoryItem SelectedItem;
    }

    /// <summary> Сигнал, отправляемый при изменении ближайшего интерактивного объекта. </summary>
    public struct ClosestInteractableChangedSignal
    {
        /// <summary> Новый ближайший объект, доступный для взаимодействия. </summary>
        public IInteractable ClosestInteractable;
    }
}