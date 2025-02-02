namespace FlavorfulStory.InventorySystem
{
    /// <summary> Слот инвентаря, содержащий предмет и его количество. </summary>
    public struct InventorySlot
    {
        /// <summary> Предмет, находящийся в этом слоте. </summary>
        public InventoryItem Item { get; set; }

        /// <summary> Количество предметов в этом слоте. </summary>
        public int Number { get; set; }
    }
}