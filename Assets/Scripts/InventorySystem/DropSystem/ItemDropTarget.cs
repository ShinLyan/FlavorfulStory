using FlavorfulStory.InventorySystem.UI.Dragging;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.DropSystem
{
    /// <summary> Целевой контейнер для предметов, выбрасываемых из инвентаря. </summary>
    public class ItemDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        /// <summary> Выбрасыватель предметов. </summary>
        private ItemDropper _itemDropper;

        /// <summary> Внедренеие зависимостей Zenject. </summary>
        /// <param name="itemDropper"> Выбрасыватель предметов. </param>
        [Inject]
        private void Construct(ItemDropper itemDropper) => _itemDropper = itemDropper;

        /// <summary> Получить максимально допустимое количество элементов,
        /// которые можно добавить в это место назначения. </summary>
        /// <remarks> Если ограничений нет, метод должен возвращать значение <c>Int.MaxValue</c>. </remarks>
        /// <param name="item"> Тип элемента, который потенциально может быть добавлен. </param>
        /// <returns> Максимально допустимое количество элементов. </returns>
        public int GetMaxAcceptableItemsNumber(InventoryItem item) => int.MaxValue;

        /// <summary> Добавить элементы в это место назначения с обновлением UI и данных. </summary>
        /// <param name="item"> Тип добавляемого элемента. </param>
        /// <param name="number"> Количество добавляемых элементов. </param>
        public void AddItems(InventoryItem item, int number) => _itemDropper.DropItem(item, number);
    }
}