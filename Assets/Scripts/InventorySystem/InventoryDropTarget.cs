using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.InventorySystem.UI.Dragging;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Целевой контейнер для предметов, выбрасываемых из инвентаря. </summary>
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        /// <summary> Получить максимально допустимое количество элементов,
        /// которые можно добавить в это место назначения. </summary>
        /// <remarks> Если ограничений нет, метод должен возвращать значение <c>Int.MaxValue</c>. </remarks>
        /// <param name="item"> Тип элемента, который потенциально может быть добавлен. </param>
        /// <returns> Максимально допустимое количество элементов. </returns>
        public int GetMaxAcceptableItemsNumber(InventoryItem item) => int.MaxValue;

        /// <summary> Добавить элементы в это место назначения с обновлением UI и данных. </summary>
        /// <param name="item"> Тип добавляемого элемента. </param>
        /// <param name="number"> Количество добавляемых элементов. </param>
        public void AddItems(InventoryItem item, int number)
        {
            var itemDropper = GameObject.FindWithTag("Player").GetComponent<ItemDropper>();
            itemDropper.DropItem(item, number);
        }
    }
}