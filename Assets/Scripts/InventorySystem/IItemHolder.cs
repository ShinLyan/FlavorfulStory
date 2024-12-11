namespace FlavorfulStory.InventorySystem
{
    /// <summary>  лассы, реализующие этот интерфейс, позвол€ют 
    /// "ItemTooltipSpawner" отображать нужную информацию.</summary>
    public interface IItemHolder
    {
        public InventoryItem GetItem();
    }
}