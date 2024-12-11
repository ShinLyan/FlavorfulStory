namespace FlavorfulStory.InventorySystem
{
    public struct InventorySlot
    {
        /// <summary> Предмет, который может быть помещен в инвентарь.</summary>
        public InventoryItem Item { get; set; }

        /// <summary> Количество предметов в инвентаре.</summary>
        public int Number { get; set; }
    }
}
