using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.InventorySystem.UI.Dragging;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Целевой контейнер для предметов, выбрасываемых из инвентаря. </summary>
    public class ItemDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        /// <summary> Ссылка на <see cref="ItemDropper"/> игрока. </summary>
        private ItemDropper _itemDropper;

        /// <summary> Поиск и снятие с игрока компонента <see cref="ItemDropper"/>. </summary>
        private void Awake() => _itemDropper = GameObject.FindWithTag("Player").GetComponent<ItemDropper>();

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