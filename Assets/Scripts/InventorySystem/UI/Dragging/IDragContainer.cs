namespace FlavorfulStory.InventorySystem.UI.Dragging
{
    /// <summary> Классы, реализующие этот интерфейс, могут выступать
    /// как в качестве источника, так и места назначения для перетаскивания "DragItem". </summary>
    /// <remarks> Если мы перетаскиваем между двумя контейнерами, то можно поменять местами элементы. </remarks>
    /// <typeparam name="T"> Тип, представляющий перетаскиваемый элемент. </typeparam>
    public interface IDragContainer<T> : IDragDestination<T>, IDragSource<T> where T : class
    {
    }
}