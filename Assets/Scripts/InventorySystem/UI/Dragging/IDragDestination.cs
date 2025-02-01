namespace FlavorfulStory.InventorySystem.UI.Dragging
{
    /// <summary> Интерфейс для классов, которые могут выступать
    /// в качестве места назначения для перетаскивания "DragItem". </summary>
    /// <typeparam name="T"> Тип объекта, который может быть перетаскиваемым. </typeparam>
    public interface IDragDestination<T> where T : class
    {
        /// <summary> Получить максимально допустимое количество элементов,
        /// которые можно добавить в это место назначения. </summary>
        /// <remarks> Если ограничений нет, метод должен возвращать значение <c>Int.MaxValue</c>. </remarks>
        /// <param name="item"> Тип элемента, который потенциально может быть добавлен. </param>
        /// <returns> Максимально допустимое количество элементов. </returns>
        public int GetMaxAcceptableItemsNumber(T item);
        
        /// <summary> Добавить элементы в это место назначения с обновлением UI и данных. </summary>
        /// <param name="item"> Тип добавляемого элемента. </param>
        /// <param name="number"> Количество добавляемых элементов. </param>
        public void AddItems(T item, int number);
    }
}