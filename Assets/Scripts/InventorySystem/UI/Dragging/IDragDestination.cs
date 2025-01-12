namespace FlavorfulStory.InventorySystem.UI.Dragging
{
    /// <summary> Классы, реализующие этот интерфейс, могут выступать
    /// в качестве места назначения для перетаскивания "DragItem". </summary>
    /// <typeparam name="T"> Тип, представляющий перетаскиваемый элемент. </typeparam>
    public interface IDragDestination<T> where T : class
    {
        /// <summary> Получить максимально допустимое количество элементов. </summary>
        /// <remarks> Если ограничения нет, то должно быть возвращено значение Int.MaxValue. </remarks>
        /// <param name="item">Тип элемента, который потенциально может быть добавлен. </param>
        /// <returns> Возвращает максимально допустимое количество элементов. </returns>
        public int GetMaxAcceptableItemsNumber(T item);

        /// <summary> Обновить UI и все данные для отображения добавления элемента в это место назначения. </summary>
        /// <param name="item">Тип элемента. </param>
        /// <param name="number">Количество элементов. </param>
        public void AddItems(T item, int number);
    }
}