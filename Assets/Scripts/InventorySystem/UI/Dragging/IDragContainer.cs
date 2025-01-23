namespace FlavorfulStory.InventorySystem.UI.Dragging
{
    /// <summary> Интерфейс для классов, которые могут быть источником и местом назначения для перетаскивания "DragItem". </summary>
    /// <remarks> Поддерживает возможность обмена элементами между двумя контейнерами при перетаскивании. </remarks>
    /// <typeparam name="T"> Тип объекта, который может быть перетаскиваемым. </typeparam>
    public interface IDragContainer<T> : IDragDestination<T>, IDragSource<T> where T : class
    {
    }
}