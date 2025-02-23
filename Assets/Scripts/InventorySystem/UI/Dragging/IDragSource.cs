namespace FlavorfulStory.InventorySystem.UI.Dragging
{
    /// <summary> Интерфейс для классов, которые могут выступать
    /// в качестве источника для перетаскивания "DragItem". </summary>
    /// <typeparam name="T"> Тип объекта, который может быть перетаскиваемым. </typeparam>
    public interface IDragSource<out T> where T : class
    {
        /// <summary> Получить элемент, который находится в этом источнике. </summary>
        /// <returns> Элемент, находящийся в данном источнике. </returns>
        public T GetItem();
        
        /// <summary> Получить количество элементов в источнике. </summary>
        /// <returns> Количество элементов в источнике. </returns>
        public int GetNumber();

        /// <summary> Удалить указанное количество элементов из источника. </summary>
        /// <param name="number"> Количество удаляемых элементов. </param>
        /// <remarks> Параметр <paramref name="number"/> не должен превышать количество,
        /// возвращаемое методом <c>GetNumber</c>. </remarks>
        public void RemoveItems(int number);
    }
}