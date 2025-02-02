namespace FlavorfulStory.InventorySystem
{
    /// <summary> Интерфейс для классов, которые предоставляют информацию для "ItemTooltipSpawner". </summary>
    public interface IItemHolder
    {
        /// <summary> Получить предмет, который содержится в объекте. </summary>
        /// <returns> Предмет инвентаря, который содержится в объекте. </returns>
        public InventoryItem GetItem();
    }
}