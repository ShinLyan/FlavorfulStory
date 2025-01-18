namespace FlavorfulStory.InventorySystem
{
    /// <summary> Классы, реализующие этот интерфейс, позволяют 
    /// "ItemTooltipSpawner" отображать нужную информацию. </summary>
    public interface IItemHolder
    {
        public InventoryItem GetItem();
    }
}