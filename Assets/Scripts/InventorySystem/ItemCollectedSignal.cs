namespace FlavorfulStory.InventorySystem
{
    /// <summary> Сигнал, отправляемый при сборе предмета в инвентарь. </summary>
    public struct ItemCollectedSignal
    {
        /// <summary> Собранный предмет и его количество. </summary>
        public ItemStack ItemStack { get; }

        /// <summary> Конструктор сигнала с указанием собранного предмета. </summary>
        /// <param name="itemStack"> Информация о собранном предмете. </param>
        public ItemCollectedSignal(ItemStack itemStack) => ItemStack = itemStack;
    }
}