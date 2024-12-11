namespace FlavorfulStory.InventorySystem.UI.Dragging
{
    /// <summary> Классы, реализующие этот интерфейс, могут выступать 
    /// в качестве источника для перетаскивания "DragItem".</summary>
    /// <typeparam name="T"> Тип, представляющий перетаскиваемый элемент.</typeparam>
    public interface IDragSource<T> where T : class
    {
        /// <summary> Получить предмет, который в данный момент находится в этом источнике.</summary>
        /// <returns> Возвращает предмет, который в данный момент находится в этом источнике.</returns>
        public T GetItem();

        /// <summary> Получить количество предметов.</summary>
        /// <returns> Возвращает количество предметов.</returns>
        public int GetNumber();

        /// <summary> Удалить заданное количество предметов из источника.</summary>
        /// <param name="number"> Количество предметов, которое необходимо удалить.</param>
        /// <remarks> Значение number не должно превышать число, возвращаемое с помощью "GetNumber".</remarks>
        public void RemoveItems(int number);
    }
}