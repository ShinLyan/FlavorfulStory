namespace FlavorfulStory.InventorySystem
{
    /// <summary> Сигнал, указывающий на необходимость употребить выбранный предмет. </summary>
    public readonly struct ConsumeSelectedItemSignal
    {
        /// <summary> Количество единиц предмета для потребления. </summary>
        public int Amount { get; }

        /// <summary> Создаёт новый сигнал потребления предмета. </summary>
        /// <param name="amount"> Количество единиц предмета, которое нужно употребить. </param>
        public ConsumeSelectedItemSignal(int amount) => Amount = amount;
    }
}